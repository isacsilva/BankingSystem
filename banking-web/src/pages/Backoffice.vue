<template>
  <q-page padding>
    <div class="row items-center justify-between q-pa-md">
      <q-btn label="Sair" color="secondary" icon="arrow_back" @click="logout" />
      <q-btn
        label="Criar nova conta"
        color="secondary"
        class="q-ml-md"
        @click="createAccount"
      />
    </div>

    <q-card class="q-pa-md q-mb-md">
      <div class="text-h5 q-mb-md text-primary">Contas Bancárias</div>
      <q-separator class="q-my-md" />
      <q-card-section class="row q-col-gutter-md">
        <q-input v-model="filterNumber" label="Número da Conta" filled />
        <q-input v-model="filterBranch" label="Agência" filled />
        <q-input v-model="filterDocument" label="Documento do Titular" filled />
        <q-btn
          label="Filtrar"
          color="primary"
          @click="applyFilters"
          class="q-ml-md"
        />
        <q-btn
          label="Limpar"
          color="primary"
          outline
          @click="clearFilters"
          class="q-ml-sm"
        />
      </q-card-section>
    </q-card>

    <q-table
      :rows="accounts"
      :columns="columns"
      row-key="id"
      flat
      bordered
      :loading="loading"
    >
      <template v-slot:body="props">
        <q-tr :props="props">
          <q-td
            v-for="col in props.cols"
            :key="col.name"
            :props="props"
            :class="col.align ? `text-${col.align}` : 'text-left'"
          >
            <template v-if="col.name === 'actions'">
              <q-btn
                dense
                icon="visibility"
                size="sm"
                flat
                @click="viewDetails(props.row)"
              >
                <q-tooltip>Exibir detalhes</q-tooltip>
              </q-btn>
              <q-btn
                dense
                icon="list"
                size="sm"
                flat
                @click="openTransactions(props.row.id)"
              >
                <q-tooltip>Abrir transações</q-tooltip>
              </q-btn>
            </template>

            <template v-else>
              {{ col.value }}
            </template>
          </q-td>
        </q-tr>

        <q-tr v-if="expanded[props.row.id]" class="bg-grey-2">
          <q-td colspan="100%">
            <div>
              <p class="text-bold">Transações:</p>
              <div class="row q-col-gutter-md items-center q-mb-md">
                <q-input
                  filled
                  v-model="filters[props.row.id].from"
                  label="Data Inicial"
                  type="date"
                  style="max-width: 200px"
                />
                <q-input
                  filled
                  v-model="filters[props.row.id].to"
                  label="Data Final"
                  type="date"
                  style="max-width: 200px"
                />
                <q-select
                  filled
                  v-model="filters[props.row.id].type"
                  :options="transactionTypes"
                  label="Tipo da Transação"
                  dense
                  style="min-width: 200px"
                />
                <q-input
                  filled
                  v-model="filters[props.row.id].counterpartyDocument"
                  label="Documento Contraparte"
                  style="max-width: 200px"
                />
                <q-btn
                  label="Filtrar"
                  color="primary"
                  @click="applyTransactionFilters(props.row.id)"
                  class="q-ml-md"
                />
                <q-btn
                  label="Limpar"
                  color="primary"
                  outline
                  @click="clearTransactionFilters(props.row.id)"
                  class="q-ml-sm"
                />
              </div>

              <q-markup-table dense flat bordered>
                <thead>
                  <tr>
                    <th style="width: 60px; text-align: center">ID</th>
                    <th style="width: 150px; text-align: left">Tipo</th>
                    <th style="width: 120px; text-align: right">Valor</th>
                    <th style="width: 200px; text-align: left">Data e Hora</th>
                    <th style="width: 180px; text-align: left">
                      Documento da Contraparte
                    </th>
                    <th style="width: 80px; text-align: center">Ações</th>
                  </tr>
                </thead>
                <tbody>
                  <tr
                    v-for="tx in getValidTransactions(props.row.id)"
                    :key="tx.id"
                  >
                    <td class="text-center">{{ tx.id }}</td>
                    <td>{{ tx.type }}</td>
                    <td class="text-right">
                      R$ {{ tx.amount?.toFixed(2) ?? "0,00" }}
                    </td>
                    <td>
                      {{
                        tx.createdAt
                          ? new Date(tx.createdAt).toLocaleString()
                          : "Data inválida"
                      }}
                    </td>
                    <td>{{ tx.counterpartyHolderDocument || "-" }}</td>
                    <td class="text-center">
                      <q-btn
                        dense
                        icon="visibility"
                        size="sm"
                        flat
                        color="primary"
                        @click="openTransactionDetails(tx)"
                      >
                        <q-tooltip>Exibir detalhes</q-tooltip>
                      </q-btn>
                    </td>
                  </tr>
                </tbody>
              </q-markup-table>

              <template
                v-if="
                  expanded[props.row.id] &&
                  getValidTransactions(props.row.id).length === 0
                "
              >
                <p class="text-grey text-italic">
                  Nenhuma transação encontrada.
                </p>
              </template>
            </div>
          </q-td>
        </q-tr>
      </template>
    </q-table>

    <TransactionDetailsModal
      v-model:show="showTransactionDialog"
      :transaction="selectedTransaction"
    />
    <BlockAmountModal
      v-model:show="showBlockAmountDialog"
      :account="selectedAccount"
      @refresh-balance="refreshBalances"
    />

    <q-dialog v-model="showAccountDialog" persistent>
      <q-card style="min-width: 500px">
        <q-card-section class="row items-center q-pa-sm bg-primary text-white">
          <div class="text-h6">Detalhes da Conta</div>
          <q-space />
          <q-btn icon="close" flat round dense v-close-popup />
        </q-card-section>

        <q-card-section v-if="selectedAccount">
          <div><strong>Conta:</strong> {{ selectedAccount.number }}</div>
          <div><strong>Agência:</strong> {{ selectedAccount.branch }}</div>
          <div><strong>Tipo:</strong> {{ selectedAccount.type }}</div>
          <div><strong>Titular:</strong> {{ selectedAccount.holderName }}</div>

          <div class="row items-center q-col-gutter-sm">
            <div><strong>Email:</strong></div>
            <q-input
              v-model="editedEmail"
              dense
              outlined
              style="max-width: 250px"
            >
              <template v-slot:append>
                <q-btn
                  icon="save"
                  color="primary"
                  dense
                  round
                  flat
                  @click="updateEmail"
                >
                  <q-tooltip>Atualizar email</q-tooltip>
                </q-btn>
              </template>
            </q-input>
          </div>

          <div>
            <strong>Documento:</strong> {{ selectedAccount.holderDocument }}
          </div>
          <div><strong>Status:</strong> {{ selectedAccount.status }}</div>

          <div class="q-mt-md">
            <strong>Saldo Disponível:</strong>
            <span v-if="typeof availableAmount === 'number'">
              R$ {{ availableAmount.toFixed(2) }}
            </span>
            <span v-else class="text-grey">Carregando...</span>
          </div>

          <div class="row items-center q-col-gutter-sm">
            <div><strong>Saldo Bloqueado:</strong></div>
            <div>
              <span v-if="typeof blockedAmount === 'number'">
                R$ {{ blockedAmount.toFixed(2) }}
              </span>
              <span v-else class="text-grey"> Carregando... </span>
            </div>
            <q-btn
              dense
              icon="lock"
              size="sm"
              flat
              color="primary"
              @click="openBlockAmountDialog(selectedAccount)"
            >
              <q-tooltip anchor="bottom middle" self="top middle"
                >Bloquear/Desbloquear Saldo</q-tooltip
              >
            </q-btn>
          </div>

          <div class="q-mt-lg row q-gutter-md justify-end">
            <q-btn
              dense
              icon="block"
              size="sm"
              color="warning"
              @click="openStatusDialog(selectedAccount)"
            >
              <q-tooltip anchor="bottom middle" self="top middle"
                >Alterar Status da Conta</q-tooltip
              >
            </q-btn>
            <q-btn
              dense
              icon="close"
              size="sm"
              color="negative"
              @click="openCloseDialog(selectedAccount)"
            >
              <q-tooltip anchor="bottom middle" self="top middle"
                >Encerrar Conta</q-tooltip
              >
            </q-btn>
          </div>
        </q-card-section>
      </q-card>
    </q-dialog>

    <q-dialog v-model="showStatusDialog" persistent>
      <q-card style="min-width: 400px">
        <q-card-section class="row items-center q-pa-sm bg-warning text-black">
          <div class="text-h6">Alterar Status da Conta</div>
          <q-space />
          <q-btn icon="close" flat round dense v-close-popup />
        </q-card-section>
        <q-card-section>
          <div class="q-mb-md">Escolha uma ação:</div>
          <div class="row q-gutter-md">
            <q-btn
              color="negative"
              label="Bloquear"
              @click="updateStatus('BLOCKED')"
            />
            <q-btn
              color="positive"
              label="Desbloquear"
              @click="updateStatus('ACTIVE')"
            />
          </div>
        </q-card-section>
      </q-card>
    </q-dialog>

    <q-dialog v-model="showCloseDialog" persistent>
      <q-card style="min-width: 400px">
        <q-card-section class="row items-center q-pa-sm bg-negative text-white">
          <q-icon name="warning" class="q-mr-sm" />
          <div class="text-h6">Encerrar Conta</div>
          <q-space />
          <q-btn icon="close" flat round dense v-close-popup />
        </q-card-section>
        <q-card-section>
          <div>
            Tem certeza que deseja encerrar a conta
            <strong>{{ selectedAccount?.number }}</strong
            >?
          </div>
        </q-card-section>
        <q-card-actions align="right">
          <q-btn flat label="Cancelar" v-close-popup />
          <q-btn
            color="negative"
            label="Encerrar"
            @click="confirmCloseAccount"
          />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import api from "../services/https-common.js";
import TransactionDetailsModal from "../components/TransactionDetailsModal.vue";
import BlockAmountModal from "../components/BlockAmountModal.vue";

const router = useRouter();

const accounts = ref([]);
const loading = ref(false);
const expanded = ref({});
const transactions = ref({});
const filters = ref({});

const showAccountDialog = ref(false);
const showStatusDialog = ref(false);
const showCloseDialog = ref(false);
const showTransactionDialog = ref(false);
const showBlockAmountDialog = ref(false);

const availableAmount = ref(0);
const blockedAmount = ref(0);
const selectedAccount = ref(null);
const selectedTransaction = ref(null);
const editedEmail = ref("");

const filterNumber = ref("");
const filterBranch = ref("");
const filterDocument = ref("");

const transactionTypes = ["CREDIT", "DEBIT", "AMOUNT_HOLD", "AMOUNT_RELEASE"];

const columns = [
  { name: "id", label: "ID", align: "left", field: "id" },
  { name: "number", label: "Conta", align: "left", field: "number" },
  { name: "branch", label: "Agência", align: "left", field: "branch" },
  { name: "holderName", label: "Titular", align: "left", field: "holderName" },
  { name: "status", label: "Status", align: "left", field: "status" },
  { name: "actions", label: "Ações", align: "center" },
];

const applyFilters = async () => {
  try {
    loading.value = true;

    const params = {};
    if (filterNumber.value) params.number = filterNumber.value;
    if (filterBranch.value) params.branch = filterBranch.value;
    if (filterDocument.value) params.document = filterDocument.value;

    const response = await api.get("/bankaccount/filter", {
      params: Object.keys(params).length > 0 ? params : undefined,
    });
    accounts.value = response.data;
  } catch (err) {
    console.error("Erro ao filtrar contas:", err);
  } finally {
    loading.value = false;
  }
};

const openTransactions = async (accountId) => {
  const isCurrentlyOpen = expanded.value[accountId];
  expanded.value = {};

  if (!isCurrentlyOpen) {
    expanded.value[accountId] = true;

    if (!filters.value[accountId]) {
      filters.value[accountId] = {
        from: "",
        to: "",
        type: "",
        counterpartyDocument: "",
      };
    }

    try {
      const response = await api.get("/transaction/filter", {
        params: { accountId },
      });
      transactions.value[accountId] = Array.isArray(response.data)
        ? response.data
        : [];
    } catch (error) {
      console.error("Erro ao carregar transações:", error);
      transactions.value[accountId] = [];
    }
  }
};

const getValidTransactions = (accountId) => {
  const txList = transactions.value[accountId];
  return Array.isArray(txList) ? txList.filter((tx) => tx) : [];
};

const applyTransactionFilters = async (accountId) => {
  try {
    const filter = filters.value[accountId] || {};
    const response = await api.get("/transaction/filter", {
      params: {
        accountId,
        from: filter.from || undefined,
        to: filter.to || undefined,
        type: filter.type || undefined,
        counterpartyDocument: filter.counterpartyDocument || undefined,
      },
    });
    transactions.value[accountId] = Array.isArray(response.data)
      ? response.data
      : [];
  } catch (error) {
    console.error("Erro ao aplicar filtros:", error);
    transactions.value[accountId] = [];
  }
};

const clearTransactionFilters = async (accountId) => {
  filters.value[accountId] = {
    from: "",
    to: "",
    type: "",
    counterpartyDocument: "",
  };

  try {
    const response = await api.get("/transaction/filter", {
      params: { accountId },
    });
    transactions.value[accountId] = Array.isArray(response.data)
      ? response.data
      : [];
  } catch (error) {
    console.error("Erro ao limpar filtros:", error);
    transactions.value[accountId] = [];
  }
};

const viewDetails = async (accountNumber) => {
  try {
    const response = await api.get(
      `/bankaccount/by-number/${accountNumber.number}`
    );
    await fetchAccountBalance(accountNumber.id);
    selectedAccount.value = response.data;
    editedEmail.value = response.data.holderEmail;
    showAccountDialog.value = true;
  } catch (error) {
    console.error("Erro ao buscar detalhes da conta:", error);
  }
};

const fetchAccountBalance = async (accountId) => {
  try {
    const response = await api.get(`/bankaccount/balance/${accountId}`);
    console.log(
      `Saldo da conta ${accountId}: R$ ${response.data.availableAmount.toFixed(
        2
      )}`
    );

    availableAmount.value = response.data.availableAmount ?? 0;
    blockedAmount.value = response.data.blockedAmount ?? 0;
  } catch (error) {
    console.error(`Erro ao buscar saldo da conta ${accountId}:`, error);
    availableAmount.value = 0;
    blockedAmount.value = 0;
  }
};

const openTransactionDetails = (transaction) => {
  selectedTransaction.value = transaction;
  showTransactionDialog.value = true;
};

const updateEmail = async () => {
  try {
    await api.put(
      `/bankaccount/update-email/${selectedAccount.value.id}`,
      null,
      {
        params: { email: editedEmail.value },
      }
    );
    selectedAccount.value.holderEmail = editedEmail.value;
    console.log("E-mail atualizado com sucesso.");
  } catch (error) {
    console.error("Erro ao atualizar e-mail:", error);
  }
};

const openStatusDialog = () => {
  showStatusDialog.value = true;
};

const updateStatus = async (newStatus) => {
  try {
    await api.put(
      `/bankaccount/update-status/${selectedAccount.value.id}`,
      null,
      {
        params: { status: newStatus },
      }
    );
    selectedAccount.value.status = newStatus;
    showStatusDialog.value = false;
    applyFilters();
  } catch (error) {
    console.error("Erro ao atualizar status da conta:", error);
  }
};

const openCloseDialog = () => {
  showCloseDialog.value = true;
};

const confirmCloseAccount = async () => {
  try {
    await api.put(`/bankaccount/close/${selectedAccount.value.id}`);
    selectedAccount.value.status = "FINISHED";
    showCloseDialog.value = false;
    applyFilters();
  } catch (error) {
    console.error("Erro ao encerrar conta:", error);
  }
};

const openBlockAmountDialog = (account) => {
  selectedAccount.value = account;
  showBlockAmountDialog.value = true;
};

const refreshBalances = async () => {
  if (selectedAccount.value?.id) {
    await fetchAccountBalance(selectedAccount.value.id);
  }
};

const clearFilters = async () => {
  filterNumber.value = "";
  filterBranch.value = "";
  filterDocument.value = "";
  await applyFilters();
};

const createAccount = () => {
  router.push("/createaccount");
};
const logout = () => {
  router.push("/login");
};

onMounted(() => {
  applyFilters();
});
</script>

<script>
export default {
  name: "BackofficePage",
};
</script>
