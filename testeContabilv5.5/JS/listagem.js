const API_CONFIG = {
    usuarios: {
        listEndpoint: 'https://localhost:7292/api/Usuario/GetUsuarios',
        getByIdEndpoint: 'https://localhost:7292/api/Usuario/GetUsuarioById', // Endpoint para buscar usuário por ID
        campos: ['id', 'nome', 'email', 'cargo', 'empresaId'],
        telaEdicao: 'cadastro_usuario.html'
    },
    empresas: {
        listEndpoint: 'https://localhost:7292/api/Empresa/GetEmpresas',
        getByIdEndpoint: 'https://localhost:7292/api/Empresa/GetEmpresaById', // Endpoint para buscar empresa por ID
        campos: ['id', 'razaoSocial', 'cnpj', 'telefone'],
        telaEdicao: 'cadastro_empresa.html'
    },
    planos: {
        listEndpoint: 'https://localhost:7292/api/ContaContabil/GetContasContabeis',
        getByIdEndpoint: 'https://localhost:7292/api/ContaContabil/GetContaContabilById', // Endpoint para buscar plano por ID
        campos: ['id', 'codigo', 'descricao', 'tipoConta', 'natureza'],
        telaEdicao: 'plano_contas.html'
    },
    historicos: {
        listEndpoint: 'https://localhost:7292/api/Historico/GetHistoricos',
        getByIdEndpoint: 'https://localhost:7292/api/Historico/GetHistoricoById', // Endpoint para buscar histórico por ID
        campos: ['id', 'descricao', 'ativo'],
        telaEdicao: 'historico.html'
    },
    lancamentos: {
        listEndpoint: 'https://localhost:7292/api/LancamentoContabil/GetLancamentosContabeis',
        getByIdEndpoint: 'https://localhost:7292/api/LancamentoContabil/GetLancamentoContabilById', // Endpoint para buscar lançamento por ID
        campos: ['id', 'data', 'valor', 'descComplementar', 'historicoId'],
        telaEdicao: 'lancamentos.html'
    }
};

let filtroAtual = null;
let token = null;

document.addEventListener('DOMContentLoaded', () => {
    token = localStorage.getItem('authToken');
    if (!token) {
        alert("Você precisa estar logado para acessar esta página.");
        window.location.href = './login.html';
        return;
    }

    document.querySelectorAll('.card-filter').forEach(card => {
        card.addEventListener('click', function () {
            const tipo = this.id.replace('filtro-', '');
            filtrarLista(tipo);
        });
    });

    document.getElementById('btn-novo').addEventListener('click', novoItem);

    carregarTodosTotais();
    document.getElementById('cabecalho-tabela').innerHTML = '';
    document.getElementById('corpo-tabela').innerHTML = `
        <tr>
            <td colspan="100%" class="px-6 py-4 text-center text-gray-500">
                Selecione um filtro acima para visualizar os detalhes.
            </td>
        </tr>
    `;
});

async function carregarTotalPorTipo(tipo) {
    try {
        const config = API_CONFIG[tipo];
        if (!config || !config.listEndpoint) { // Verifica se listEndpoint existe
            console.warn(`Configuração ou 'listEndpoint' para o tipo '${tipo}' não encontrada.`);
            return;
        }

        const response = await fetch(config.listEndpoint, { // Usa listEndpoint
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            const errorBody = await response.text();
            console.error(`Erro HTTP ao carregar total de ${tipo}: ${response.status} - ${errorBody}`);
            throw new Error(`Erro ao carregar total de ${tipo}: ${response.status}`);
        }

        const resultado = await response.json();
        const dados = resultado.dados;

        const totalSpan = document.getElementById(`total-${tipo}`);
        if (totalSpan) {
            totalSpan.textContent = dados ? dados.length : 0;
        } else {
            console.warn(`Elemento 'total-${tipo}' não encontrado para atualizar o total.`);
        }
    } catch (erro) {
        console.error(`Erro ao carregar total de ${tipo}:`, erro);
        const totalSpan = document.getElementById(`total-${tipo}`);
        if (totalSpan) {
            totalSpan.textContent = 'Erro';
            totalSpan.classList.add('text-red-500');
        }
    }
}

async function carregarTodosTotais() {
    for (const tipo in API_CONFIG) {
        if (API_CONFIG.hasOwnProperty(tipo)) {
            await carregarTotalPorTipo(tipo);
        }
    }
}

async function filtrarLista(tipo) {
    try {
        filtroAtual = tipo;

        document.querySelectorAll('.card-filter').forEach(card => {
            card.classList.remove('active', 'border-blue-500');
            card.classList.add('border-transparent');
        });
        document.getElementById(`filtro-${tipo}`).classList.add('active', 'border-blue-500');

        document.getElementById('cabecalho-tabela').innerHTML = '';
        document.getElementById('corpo-tabela').innerHTML = `
            <tr>
                <td colspan="100%" class="px-6 py-4 text-center text-gray-500">
                    Carregando dados...
                </td>
            </tr>
        `;

        const dados = await carregarDados(tipo); // carregarDados agora usa listEndpoint

        atualizarTabela(dados, tipo);

        document.getElementById(`total-${tipo}`).textContent = dados.length;

    } catch (erro) {
        console.error('Erro ao filtrar lista:', erro);
        document.getElementById('cabecalho-tabela').innerHTML = '';
        document.getElementById('corpo-tabela').innerHTML = `
            <tr>
                <td colspan="100%" class="px-6 py-4 text-center text-red-500">
                    Erro ao carregar dados. Verifique o console.
                </td>
            </tr>
        `;
    }
}

// carregarDados agora usa listEndpoint de API_CONFIG
async function carregarDados(tipo) {
    const config = API_CONFIG[tipo];
    if (!config || !config.listEndpoint) throw new Error(`Tipo ${tipo} ou seu 'listEndpoint' não configurado`);

    const response = await fetch(config.listEndpoint, {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    });

    if (response.status === 401) {
        alert('Sessão expirada ou não autorizada. Por favor, faça login novamente.');
        localStorage.removeItem('authToken');
        window.location.href = './login.html';
        throw new Error('Não autorizado');
    }

    if (!response.ok) {
        const errorBody = await response.text();
        throw new Error(`Erro HTTP: ${response.status} - ${errorBody}`);
    }

    const resultado = await response.json();
    return resultado.dados;
}

function atualizarTabela(dados, tipo) {
    const config = API_CONFIG[tipo];
    const cabecalho = document.getElementById('cabecalho-tabela');
    const corpo = document.getElementById('corpo-tabela');

    cabecalho.innerHTML = '';
    corpo.innerHTML = '';

    if (dados.length === 0) {
        corpo.innerHTML = `
            <tr>
                <td colspan="100%" class="px-6 py-4 text-center text-gray-500">
                    Nenhum registro encontrado
                </td>
            </tr>
        `;
        return;
    }

    const linhaCabecalho = document.createElement('tr');
    config.campos.forEach(campo => {
        const th = document.createElement('th');
        th.className = 'px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase';
        th.textContent = campo;
        linhaCabecalho.appendChild(th);
    });

    // Re-adiciona a coluna de Ações
    const thAcoes = document.createElement('th');
    thAcoes.className = 'px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase';
    thAcoes.textContent = 'Ações';
    linhaCabecalho.appendChild(thAcoes);

    cabecalho.appendChild(linhaCabecalho);

    dados.forEach(item => {
        const linha = document.createElement('tr');
        linha.className = 'hover:bg-gray-50';

        config.campos.forEach(campo => {
            const td = document.createElement('td');
            td.className = 'px-6 py-4 whitespace-nowrap text-sm text-gray-500 text-left !important';
            td.textContent = String(item[campo] || '-');
            linha.appendChild(td);
        });

        // Re-adiciona os botões de ação
        const tdAcoes = document.createElement('td');
        tdAcoes.className = 'px-6 py-4 whitespace-nowrap text-right text-sm font-medium';

        // Botão Visualizar
        const btnVisualizar = document.createElement('button');
        btnVisualizar.className = 'text-gray-600 hover:text-gray-900 mr-3';
        btnVisualizar.textContent = 'Visualizar';
        btnVisualizar.onclick = () => visualizarItem(tipo, item.id);
        tdAcoes.appendChild(btnVisualizar);

        // Botão Editar
        const btnEditar = document.createElement('button');
        btnEditar.className = 'text-blue-600 hover:text-blue-900 mr-3';
        btnEditar.textContent = 'Editar';
        btnEditar.onclick = () => editarItem(tipo, item.id);
        tdAcoes.appendChild(btnEditar);

        linha.appendChild(tdAcoes);
        corpo.appendChild(linha);
    });
}

// Função para editar um item - agora passa 'mode=edit'
function editarItem(tipo, id) {
    const config = API_CONFIG[tipo];
    if (!config || !config.telaEdicao) {
        console.error(`Tipo ${tipo} ou 'telaEdicao' não configurado para edição`);
        return;
    }
    // Abre em nova aba com o ID e o modo como parâmetro
    window.open(`${config.telaEdicao}?id=${id}&mode=edit`, '_blank');
}

// NOVA Função para visualizar um item - passa 'mode=view'
function visualizarItem(tipo, id) {
    const config = API_CONFIG[tipo];
    if (!config || !config.telaEdicao) {
        console.error(`Tipo ${tipo} ou 'telaEdicao' não configurado para visualização`);
        return;
    }
    // Abre em nova aba com o ID e o modo como parâmetro
    window.open(`${config.telaEdicao}?id=${id}&mode=view`, '_blank');
}

function novoItem() {
    if (!filtroAtual) {
        alert('Selecione um tipo de lista antes (ex: "Usuários")');
        return;
    }

    const config = API_CONFIG[filtroAtual];
    // Ao criar novo, não passamos ID nem modo, pois é uma tela vazia para preencher
    window.open(config.telaEdicao, '_blank');
}