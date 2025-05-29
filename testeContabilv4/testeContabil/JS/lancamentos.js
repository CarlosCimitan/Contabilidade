document.addEventListener('DOMContentLoaded', function () {
  const API_BASE = 'https://localhost:7292/api';
  window.ENDPOINTS = {
    LANCAMENTOS: `${API_BASE}/LancamentoContabil/CriarLancamentoContabil`,
    PLANO_CONTAS: `${API_BASE}/ContaContabil/GetContasContabeisById`,
    HISTORICOS: `${API_BASE}/Historico/GetHistoricos`,
    EMPRESAS: `${API_BASE}/Empresa/GetEmpresas`
  };

  // Autenticação
  const token = localStorage.getItem('authToken');
  if (!token) {
    alert('Você precisa estar logado para acessar esta página.');
    window.location.href = './login.html';
    return;
  }

  // Helper: fetch com Authorization
  async function fetchComAuth(url, options = {}) {
    options.headers = options.headers || {};
    options.headers['Content-Type'] = 'application/json';
    options.headers['Authorization'] = `Bearer ${token}`;
    return fetch(url, options);
  }

  // Inicialização
  atualizarDiaSemana();

  // Eventos principais
  document.getElementById('Data').addEventListener('change', atualizarDiaSemana);
  document.getElementById('adicionarCredito').addEventListener('click', () => clonarLinha('contasCredito'));
  document.getElementById('adicionarDebito').addEventListener('click', () => clonarLinha('contasDebito'));
  document.addEventListener('input', atualizarTotais);
  document.getElementById('Cancelar').addEventListener('click', limparFormulario);
  document.getElementById('Gravar').addEventListener('click', gravarLancamento);

  // Botões de listagem
  document.querySelectorAll('#contasCredito button, #contasDebito button').forEach(btn => {
    btn.addEventListener('click', e => {
      e.preventDefault();
      listarGenerico('Plano de Contas', window.ENDPOINTS.PLANO_CONTAS);
    });
  });

  // Menu responsivo
  document.getElementById('navbar-toggle').addEventListener('click', () => {
    document.getElementById('navbar').classList.toggle('hidden');
  });

  // Inputs das linhas de crédito e débito (busca conta por ID)
  document.querySelectorAll('#contasCredito input[type="text"]:first-child, #contasDebito input[type="text"]:first-child')
    .forEach(input => {
      input.addEventListener('change', function () {
        buscarContaContabil(this);
      });
    });

  // Preenche descrição do histórico automaticamente
document.getElementById('ContaContabilId').addEventListener('change', async function () {
  const codigo = this.value.trim();
  if (!codigo) return;

  try {
    const res = await fetch('https://localhost:7292/api/Historico/GetHistoricos', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('authToken')}`
      }
    });

    if (!res.ok) {
      if (res.status === 401) {
        alert('Sessão expirada. Faça login novamente.');
        window.location.href = './login.html';
        return;
      }
      throw new Error('Erro ao buscar históricos');
    }

    const json = await res.json();
    const historicos = json.dados || [];

    const historico = historicos.find(h => h.codigo == codigo);

    if (historico) {
      document.getElementById('DescComplementar').value = historico.descricao || '';
    } else {
      alert('Histórico não encontrado!');
      document.getElementById('DescComplementar').value = '';
    }

  } catch (error) {
    alert(`Erro ao buscar histórico: ${error.message}`);
    this.value = '';
    document.getElementById('DescComplementar').value = '';
  }
});
  // ---------------------------
  // FUNÇÕES AUXILIARES
  // ---------------------------

async function buscarContaContabil(input) {
  const codigo = input.value.trim();
  const linha = input.closest('.flex.space-x-4');

  if (!codigo) {
    limparCamposDaLinha(linha);
    return;
  }

  try {
    const response = await fetch(`${window.ENDPOINTS.PLANO_CONTAS}/${codigo}`, {
      headers: { 'Authorization': `Bearer ${token}` }
    });

    if (!response.ok) throw new Error('Conta não encontrada');

    const json = await response.json();
    const conta = json.dados?.[0];

    if (!conta) throw new Error('Nenhuma conta retornada');

    const inputs = linha.querySelectorAll('input');
    inputs[1].value = conta.mascara || '';
    inputs[2].value = conta.descricao || '';
  } catch (err) {
    alert(err.message);
    input.value = '';
    limparCamposDaLinha(linha);
  }
}


  function limparCamposDaLinha(linha) {
    const inputs = linha.querySelectorAll('input');
    if (inputs.length >= 3) {
      inputs[1].value = '';
      inputs[2].value = '';
    }
  }

  function atualizarTotais() {
    let totalCredito = 0;
    let totalDebito = 0;

    document.querySelectorAll('#contasCredito .valorCredito').forEach(input => {
      totalCredito += Number(input.value) || 0;
    });
    document.querySelectorAll('#contasDebito .valorDebito').forEach(input => {
      totalDebito += Number(input.value) || 0;
    });

    document.getElementById('TotalCredito').value = totalCredito.toFixed(2);
    document.getElementById('TotalDebito').value = totalDebito.toFixed(2);
    document.getElementById('Diferenca').value = (totalCredito - totalDebito).toFixed(2);
  }

  function validarLancamento() {
    const diferenca = Number(document.getElementById('Diferenca').value);
    if (diferenca !== 0) {
      alert("Erro: Débitos e Créditos devem ser iguais!");
      return false;
    }
    if (!document.getElementById('ContaContabilId').value) {
      alert("Código do Histórico é obrigatório!");
      return false;
    }
    return true;
  }

  async function gravarLancamento() {
    if (!validarLancamento()) return;

    const dadosLancamento = {
      zeramento: document.getElementById('Zeramento').checked,
      descComplementar: document.getElementById('DescComplementar').value,
      debitosCreditos: []
    };

    document.querySelectorAll('#contasDebito > .flex.space-x-4').forEach(linha => {
      const inputs = linha.querySelectorAll('input');
      if (inputs[0].value && inputs[3].value) {
        dadosLancamento.debitosCreditos.push({
          data: new Date().toISOString(),
          valor: Number(inputs[3].value),
          tipoAcao: 1,
          descComplementar: dadosLancamento.descComplementar,
          contaContabilId: Number(inputs[0].value)
        });
      }
    });

    document.querySelectorAll('#contasCredito > .flex.space-x-4').forEach(linha => {
      const inputs = linha.querySelectorAll('input');
      if (inputs[0].value && inputs[3].value) {
        dadosLancamento.debitosCreditos.push({
          data: new Date().toISOString(),
          valor: Number(inputs[3].value),
          tipoAcao: 2,
          descComplementar: dadosLancamento.descComplementar,
          contaContabilId: Number(inputs[0].value)
        });
      }
    });

    try {
      const res = await fetchComAuth(window.ENDPOINTS.LANCAMENTOS, {
        method: 'POST',
        body: JSON.stringify(dadosLancamento)
      });

      if (!res.ok) {
        const err = await res.json();
        throw new Error(err.message || 'Erro ao gravar lançamento');
      }

      alert('Lançamento gravado com sucesso!');
      limparFormulario();
    } catch (error) {
      alert(`Erro: ${error.message}`);
    }
  }

  function clonarLinha(containerId) {
    const container = document.getElementById(containerId);
    const linhas = container.querySelectorAll('.flex.space-x-4');
    const linhaOriginal = linhas[linhas.length - 1];
    if (!linhaOriginal) return;

    const novaLinha = linhaOriginal.cloneNode(true);
    novaLinha.querySelectorAll('input').forEach(input => {
      if (!input.readOnly) input.value = '';
    });

    const inputConta = novaLinha.querySelector('input[type="text"]:first-child');
    if (inputConta) inputConta.addEventListener('change', function () {
      buscarContaContabil(this);
    });

    const btnListagem = novaLinha.querySelector('button');
    if (btnListagem) {
      btnListagem.addEventListener('click', e => {
        e.preventDefault();
        listarGenerico('Plano de Contas', window.ENDPOINTS.PLANO_CONTAS);
      });
    }

    container.appendChild(novaLinha);
    atualizarTotais();
  }

  function limparFormulario() {
    document.querySelectorAll('input:not([readonly]), textarea').forEach(el => el.value = '');
    document.getElementById('Zeramento').checked = false;

    ['contasCredito', 'contasDebito'].forEach(containerId => {
      const container = document.getElementById(containerId);
      const linhas = container.querySelectorAll('.flex.space-x-4');
      linhas.forEach((linha, idx) => {
        if (idx > 0) linha.remove();
      });
    });

    atualizarTotais();
  }

  async function listarGenerico(titulo, endpoint) {
    try {
      const res = await fetchComAuth(endpoint);
      if (!res.ok) throw new Error('Erro ao buscar dados');
      const dados = await res.json();
      console.table(dados.dados || dados);
      alert(`${titulo} carregado. Veja o console.`);
    } catch (error) {
      alert(`Erro: ${error.message}`);
    }
  }

  function atualizarDiaSemana() {
    const dataStr = document.getElementById('Data').value;
    if (!dataStr) return;
    const data = new Date(dataStr);
    const diaSemana = data.toLocaleDateString('pt-BR', { weekday: 'long' });
    document.getElementById('DiaSemana').value = diaSemana.charAt(0).toUpperCase() + diaSemana.slice(1);
  }
});
