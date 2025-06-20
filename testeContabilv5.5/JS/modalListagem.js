// modalListagem.js
document.addEventListener('DOMContentLoaded', function () {
    const API_CONFIG = {
        usuarios: {
            listEndpoint: 'https://localhost:7292/api/Usuario/GetUsuarios',
            campos: ['id', 'nome', 'email', 'cargo', 'empresaId'],
            headers: ['ID', 'Nome', 'Email', 'Cargo', 'Empresa ID']
        },
        empresas: {
            listEndpoint: 'https://localhost:7292/api/Empresa/GetEmpresas',
            campos: ['id', 'razaoSocial', 'cnpj', 'telefone'],
            headers: ['ID', 'Razão Social', 'CNPJ', 'Telefone']
        },
        planos: {
            listEndpoint: 'https://localhost:7292/api/ContaContabil/GetContasContabeis',
            campos: ['id', 'codigo', 'descricao', 'tipoConta', 'natureza'],
            headers: ['ID', 'Código', 'Descrição', 'Tipo', 'Natureza']
        },
        historicos: {
            listEndpoint: 'https://localhost:7292/api/Historico/GetHistoricos',
            campos: ['id', 'descricao', 'ativo'],
            headers: ['ID', 'Descrição', 'Ativo']
        },
        lancamentos: {
            listEndpoint: 'https://localhost:7292/api/LancamentoContabil/GetLancamentosContabeis',
            campos: ['id', 'data', 'valor', 'descComplementar', 'historicoId'],
            headers: ['ID', 'Data', 'Valor', 'Descrição', 'Histórico ID']
        }
    };



    // Cria o modal dinamicamente
    const modalHTML = `
        <div id="listagemModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full hidden z-50">
            <div class="relative top-20 mx-auto p-5 border w-11/12 md:w-3/4 lg:w-2/3 shadow-lg rounded-md bg-white">
                <div class="flex justify-between items-center border-b pb-3">
                    <h3 class="text-xl font-semibold text-gray-800" id="modalTitle">Listagem</h3>
                    <button id="closeModal" class="text-gray-500 hover:text-gray-700">
                        <svg class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>
                <div class="mt-4 mb-6 overflow-x-auto">
                    <div id="loadingIndicator" class="animate-pulse flex space-x-4">
                        <div class="flex-1 space-y-4 py-1">
                            <div class="h-4 bg-gray-300 rounded w-3/4"></div>
                            <div class="space-y-2">
                                <div class="h-4 bg-gray-300 rounded"></div>
                                <div class="h-4 bg-gray-300 rounded w-5/6"></div>
                            </div>
                        </div>
                    </div>
                    <div id="modalContent" class="hidden">
                        <div class="mb-4 flex justify-between items-center">
                            <div class="text-sm text-gray-600" id="totalItens"></div>
                            <div class="relative">
                                <input type="text" id="searchInput" placeholder="Pesquisar..." class="pl-8 pr-4 py-2 border rounded-md text-sm">
                                <svg class="absolute left-2.5 top-2.5 h-4 w-4 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                                </svg>
                            </div>
                        </div>
                        <table class="min-w-full divide-y divide-gray-200">
                            <thead class="bg-gray-50" id="tableHeader"></thead>
                            <tbody class="bg-white divide-y divide-gray-200" id="tableBody"></tbody>
                        </table>
                        <div class="mt-4 flex items-center justify-between" id="paginationControls"></div>
                    </div>
                    <div id="errorMessage" class="hidden text-red-500 p-4"></div>
                </div>
                <div class="flex justify-end pt-2 border-t">
                    <button id="confirmModal" class="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600">Fechar</button>
                </div>
            </div>
        </div>
    `;

    let ultimoBotaoClicado = null;
    // Adiciona o modal ao body
    document.body.insertAdjacentHTML('beforeend', modalHTML);

    // Variáveis para controle de paginação
    let currentPage = 1;
    const itemsPerPage = 10;
    let currentData = [];
    let currentType = '';

    // Event listeners para abrir/fechar o modal
    document.addEventListener('click', function (e) {
        // Mapeamento de botões para tipos
        const buttonToTypeMap = {
            'btnListagem': null, // Será determinado pela página
            'btnListagemPlanos': 'planos',
            'btnListagemHistorico': 'historicos',
            'btnListagemUsuario': 'usuarios',
            'btnListagemLancamentos': 'lancamentos',
            'btnListagemEmpresa': 'empresas',
            'btnListagemZeramento': 'lancamentos'
        };

        // Verifica se algum botão foi clicado
        let clickedButton = null;
        for (const [buttonId, type] of Object.entries(buttonToTypeMap)) {
            if (e.target.id === buttonId || e.target.closest(`#${buttonId}`)) {
                clickedButton = buttonId;
                break;
            }
        }

        if (clickedButton) {
            const token = localStorage.getItem('authToken');
            if (!token) {
                alert("Você precisa estar logado para acessar esta página.");
                window.location.href = './login.html';
                return;
            }

            // Determina o tipo com base no botão clicado
            let listType = buttonToTypeMap[clickedButton];

            // Se for o botão genérico, determina o tipo pela página
            if (clickedButton === 'btnListagem') {
                const page = window.location.pathname.split('/').pop().replace('.html', '');
                listType = getTypeFromPage(page);
            }

            if (!listType) {
                showError('Tipo de listagem não configurado');
                return;
            }

            openModal(listType);
        }


        if (e.target.id === 'closeModal' || e.target.id === 'confirmModal' || e.target === document.getElementById('listagemModal')) {
            closeModal();
        }
    });

    document.getElementById('listagemModal').addEventListener('click', function (e) {
        if (e.target === this) {
            closeModal();
        }
    });

    // Mapear páginas para tipos
    function getTypeFromPage(page) {
        const pageToType = {
            'cadastro_usuario': 'usuarios',
            'cadastro_empresa': 'empresas',
            'plano_contas': 'planos',
            'historico': 'historicos',
            'lancamentos': 'lancamentos',
            'zeramento': 'lancamentos'
        };
        return pageToType[page] || null;
    }

    // Abrir o modal
    async function openModal(type) {
        currentPage = 1;
        currentType = type;
        const modal = document.getElementById('listagemModal');
        modal.classList.remove('hidden');
        document.getElementById('modalTitle').textContent = `Listagem de ${type.charAt(0).toUpperCase() + type.slice(1)}`;

        showLoading();
        hideError();
        hideContent();

        try {
            const data = await loadData(type);
            currentData = data;
            renderTable(data, type);
            setupPagination(data.length);
            setupSearch();
        } catch (error) {
            console.error('Erro ao carregar dados:', error);
            showError('Erro ao carregar a listagem. Tente novamente mais tarde.');
        }
    }

    //Fechar o modal
    function closeModal() {
        document.getElementById('listagemModal').classList.add('hidden');
        currentData = [];
        currentType = '';
    }




    // Carregar dados da API
    async function loadData(type) {
        const config = API_CONFIG[type];
        if (!config || !config.listEndpoint) {
            throw new Error(`Configuração para o tipo '${type}' não encontrada.`);
        }

        const token = localStorage.getItem('authToken');
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
        return resultado.dados || [];
    }

    // Renderizar a tabela
    function renderTable(data, type) {
        const config = API_CONFIG[type];
        const header = document.getElementById('tableHeader');
        const body = document.getElementById('tableBody');

        header.innerHTML = '';
        body.innerHTML = '';

        // Criar cabeçalho
        const headerRow = document.createElement('tr');
        config.headers.forEach(headerText => {
            const th = document.createElement('th');
            th.className = 'px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider';
            th.textContent = headerText;
            headerRow.appendChild(th);
        });
        header.appendChild(headerRow);

        // Criar linhas do corpo
        const startIndex = (currentPage - 1) * itemsPerPage;
        const paginatedData = data.slice(startIndex, startIndex + itemsPerPage);

        if (paginatedData.length === 0) {
            const row = document.createElement('tr');
            const cell = document.createElement('td');
            cell.className = 'px-6 py-4 text-center text-gray-500';
            cell.colSpan = config.headers.length;
            cell.textContent = 'Nenhum registro encontrado';
            row.appendChild(cell);
            body.appendChild(row);
        } else {
            paginatedData.forEach(item => {
                const row = document.createElement('tr');
                row.className = 'hover:bg-gray-50 cursor-pointer';
                row.dataset.item = JSON.stringify(item);

                config.campos.forEach(campo => {
                    const cell = document.createElement('td');
                    cell.className = 'px-6 py-4 whitespace-nowrap text-sm text-gray-500';

                    let value = item[campo] || '-';
                    if (campo === 'data') {
                        value = new Date(value).toLocaleDateString();
                    } else if (campo === 'ativo') {
                        value = value ? 'Ativo' : 'Inativo';
                    } else if (campo === 'cnpj') {
                        value = formatCNPJ(value);
                    }

                    cell.textContent = value;
                    row.appendChild(cell);
                });

                row.addEventListener('click', () => {
                    fillFormFields(item);
                    closeModal();
                });

                body.appendChild(row);
            });
        }

        document.getElementById('totalItens').textContent = `Total de itens: ${data.length}`;
        hideLoading();
        showContent();
    }

    // Preencher os campos do formulário
    function safeFill(context, selector, value) {
        const element = (typeof context === 'string' ? document : context).querySelector(selector);
        if (element) {
            element.value = value || '';

            ['change', 'input', 'blur'].forEach(eventType => {
                element.dispatchEvent(new Event(eventType, { bubbles: true }));
            });
        }
    }

    function fillFormFields(itemData) {
        try {
            // Identificação botão clicado
            const clickedButton = document.activeElement ||
                (document.querySelector(':focus') && document.querySelector(':focus').closest('button'));

            console.log('Debug:', {
                currentType: currentType,
                clickedButton: clickedButton ? {
                    id: clickedButton.id,
                    dataset: clickedButton.dataset
                } : null,
                itemData: itemData
            });

            if (!clickedButton) {
                console.error('Nenhum botão clicado detectado');
                return;
            }

            const isLancamento = window.location.pathname.includes('lancamento');



            // Lógica para EMPRESA

            if (currentType === 'empresas') {

                safeFill(document, '#nomeEmpresa', itemData.razaoSocial);
                safeFill(document, '#cnpj', itemData.cnpj);

                // Chama a função global para preparar edição
                if (window.carregarEmpresaParaEdicao) {
                    window.carregarEmpresaParaEdicao(itemData);
                }
                return;
            }



            // 2. Lógica para USUARIO
            if (currentType === 'usuarios') {
                safeFill(document, '#codigoUsuario', itemData.id || '');
                safeFill(document, '#nomeUsuario', itemData.nome || '');
                safeFill(document, '#emailUsuario', itemData.email || '');
                safeFill(document, '#tipoUsuario', itemData.cargo || '');
                safeFill(document, '#empresa', itemData.empresaId || '');



                document.querySelector('h2').textContent = `Editando Usuário: ${itemData.nome || ''}`;
                return;
            }

            // Lógica para LANÇAMENTOS
            if (currentType === 'lancamentos') {

                //  Limpa campos básicos
                document.getElementById('Id').value = '';
                document.getElementById('descComplementarLancamento').value = '';
                document.getElementById('historicoIdLancamento').value = '';

                //  Limpa linhas dinâmicas (exceto a primeira de cada tipo)
                ['Credito', 'Debito'].forEach(tipo => {
                    const container = document.getElementById(`contas${tipo}`);
                    const linhas = container.querySelectorAll('.linha-conta');

                    // Remove linhas adicionais
                    linhas.forEach((linha, idx) => {
                        if (idx > 0) linha.remove();
                    });

                    // Limpa campos da primeira linha
                    if (linhas[0]) {
                        linhas[0].querySelector('[data-conta-input="id"]').value = '';
                        linhas[0].querySelector('[data-conta-input="descricao"]').value = '';
                        linhas[0].querySelector('[data-conta-input="mascara"]').value = '';
                        linhas[0].querySelector(`.valor${tipo}`).value = '';
                    }
                });


                // Preenche dados básicos
                safeFill(document, '#Id', itemData.id);
                safeFill(document, '#descComplementarLancamento', itemData.descComplementar || '');
                safeFill(document, '#historicoIdLancamento', itemData.historicoId || '');

                // Preenche contas (débitos e créditos)
                itemData.debitosCreditos?.forEach((item, index) => {
                    const isCredito = item.tipoAcao === 1; // 1=Crédito, 2=Débito
                    const containerId = isCredito ? 'contasCredito' : 'contasDebito';
                    const container = document.getElementById(containerId);

                    if (!container) {
                        console.error(`Container ${containerId} não encontrado`);
                        return;
                    }

                    // Obter todas as linhas
                    const rows = container.querySelectorAll('.linha-conta');
                    let row = rows[index];


                    // Se ainda não tem linha, usa a primeira
                    if (!row && rows[0]) {
                        row = rows[0];
                    }

                    if (row) {
                        // Preenche campos da conta
                        const fields = {
                            id: row.querySelector('[data-conta-input="id"]'),
                            descricao: row.querySelector('[data-conta-input="descricao"]'),
                            mascara: row.querySelector('[data-conta-input="mascara"]'),
                            valor: row.querySelector(isCredito ? '.valorCredito' : '.valorDebito')
                        };

                        if (fields.id) {
                            fields.id.value = item.contaContabil?.id || '';
                            // Dispara eventos para buscar automaticamente a descrição e máscara
                            fields.id.dispatchEvent(new Event('change'));
                            fields.id.dispatchEvent(new Event('blur'));
                        }
                        if (fields.valor) fields.valor.value = item.valor || '';
                    }
                });
                return;
            }

            // 3. Lógica para PLANOS (Versão 100% testada)

            if (currentType === 'planos') {
                // Zeramento (mantém inalterado)
                if (isZeramento) {
                    safeFill(document, '#contaResultadoId', itemData.id);
                    safeFill(document, '#contaResultadoDesc', itemData.descricao);
                    return;
                }

                // Lançamentos - mantém a lógica existente
                if (isLancamento) {
                    const isCredito = clickedButton.id.includes('Credito');
                    const containerId = isCredito ? 'contasCredito' : 'contasDebito';
                    const container = document.getElementById(containerId);

                    if (!container) {
                        console.error('Container não encontrado:', containerId);
                        return;
                    }

                    const rows = container.querySelectorAll('.linha-conta');
                    const row = rows[rows.length - 1];

                    if (row) {
                        row.querySelector('[data-conta-input="id"]').value = itemData.id || '';
                        row.querySelector('[data-conta-input="descricao"]').value = itemData.descricao || '';
                        row.querySelector('[data-conta-input="mascara"]').value = itemData.codigo || '';

                        const valorClass = isCredito ? 'valorCredito' : 'valorDebito';
                        const valorInput = row.querySelector('.' + valorClass);
                        if (valorInput) valorInput.focus();
                    }
                    return;
                }

                // Página principal do Plano de Contas - NOVA IMPLEMENTAÇÃO
                if (window.location.pathname.includes('plano_contas')) {
                    // Preenche todos os campos do formulário
                    safeFill(document, '#Codigo', itemData.codigo);
                    safeFill(document, '#Mascara', itemData.mascara);
                    safeFill(document, '#Natureza', itemData.natureza);
                    safeFill(document, '#TipoConta', itemData.tipoConta);
                    safeFill(document, '#Grau', itemData.grau);
                    safeFill(document, '#Descricao', itemData.descricao);

                    // Preenche os checkboxes de relatórios (ajustar conforme API)
                    // safeSetCheckbox(document, '#livroDiario', itemData.livroDiario);
                    // safeSetCheckbox(document, '#balancoPatrimonial', itemData.balancoPatrimonial);
                    // safeSetCheckbox(document, '#balancete', itemData.balancete);

                    // Armazena o ID para edição
                    if (window.carregarContaParaEdicao) {
                        window.carregarContaParaEdicao(itemData);
                    }
                    return;
                }
            }

            function safeSetCheckbox(context, selector, value) {
                const element = (typeof context === 'string' ? document : context).querySelector(selector);
                if (element) {
                    element.checked = Boolean(value);
                }
            }
            // 4. Lógica para HISTÓRICOS - Página de Cadastro de Histórico
            if (currentType === 'historicos') {
                // Página principal de histórico (cadastro_historico.html)
                if (window.location.pathname.includes('historico') &&
                    !window.location.pathname.includes('zeramento') &&
                    !window.location.pathname.includes('lancamento')) {

                    safeFill(document, '#historicoId', itemData.id);
                    safeFill(document, '#codigoHistorico', itemData.id);
                    safeFill(document, '#descricaoHistorico', itemData.descricao);
                }
                // Zeramento
                else if (window.location.pathname.includes('zeramento')) {
                    safeFill(document, '#historicoIdZeramento', itemData.id);
                    safeFill(document, '#descComplementarZeramento', itemData.descricao);
                }
                // Lançamentos (ATUALIZADO para usar os seletores corretos)
                else if (window.location.pathname.includes('lancamento')) {
                    safeFill(document, '#historicoIdLancamento', itemData.id);
                    safeFill(document, '#descComplementarLancamento', itemData.descricao);

                    // Dispara eventos para garantir que qualquer validação seja acionada
                    const historicoInput = document.getElementById('historicoIdLancamento');
                    if (historicoInput) {
                        historicoInput.dispatchEvent(new Event('change'));
                        historicoInput.dispatchEvent(new Event('blur'));
                    }
                }
                return;
            }

            // 5. Fallback com mapeamentos originais
            const fieldMappings = {
                usuarios: {
                    'id': 'codigoUsuario',
                    'nome': 'Nome do Usuário',
                    'email': 'Email',
                    'cargo': 'Tipo de Usuário',
                    'empresaId': 'Empresa'
                },
                empresas: {
                    'id': 'codigoEmpresa',
                    'razaoSocial': 'Nome da Empresa',
                    'cnpj': 'CNPJ',
                    'telefone': 'Telefone'
                },
                planos: {
                    'codigo': 'Código Reduzido',
                    'descricao': 'Descrição',
                    'tipoConta': 'Tipo',
                    'natureza': 'Natureza'
                },
                lancamentos: {
                    'data': 'Data de Lançamento',
                    'descComplementar': 'Descrição Complementar',
                    'historicoId': 'Código ID Histórico',
                    'valor': 'Valor'
                },
                historicos: {
                    'descricao': 'Descrição Complementar',
                    'ativo': 'Situação'
                }
            };

            // Aplica o mapeamento padrão
            const mapping = fieldMappings[currentType] || {};
            for (const [apiField, formField] of Object.entries(mapping)) {
                const input = findInputByLabel(formField);
                if (input) {
                    input.value = itemData[apiField] || '';
                    const event = new Event('change', { bubbles: true });
                    input.dispatchEvent(event);
                }
            }

            // 6. Atualiza campos de ID ocultos
            if (itemData.id) {
                const idField = document.getElementById(`${currentType}Id`);
                if (idField) idField.value = itemData.id;
            }

        } catch (error) {
            console.error('Erro no fillFormFields:', error);
            // Logs detalhados para depuração
            console.log("Tipo atual:", currentType);
            console.log("Página atual:", window.location.pathname);
            console.log("Botão clicado:", clickedButton ? clickedButton.id : 'N/A');
            console.log("Dados recebidos:", itemData);
        }
    }

    // Funções auxiliares
    function safeSetValue(context, selector, value) {
        const element = (typeof context === 'string' ? document : context).querySelector(selector);
        if (element) {
            element.value = value || '';
            dispatchEvents(element);
        }
    }

    function dispatchEvents(element) {
        ['change', 'input'].forEach(eventType => {
            element.dispatchEvent(new Event(eventType, { bubbles: true }));
        });
    }

    // Função auxiliar para encontrar inputs por label
    function findInputByLabel(labelText) {
        // Primeiro tenta encontrar pelo texto da label
        const labels = Array.from(document.querySelectorAll('label'));
        const label = labels.find(el => {
            const text = el.textContent.trim();
            return text.includes(labelText) ||
                labelText.includes(text) ||
                text.toLowerCase().includes(labelText.toLowerCase());
        });

        if (label) {
            const inputId = label.getAttribute('for');
            if (inputId) {
                return document.getElementById(inputId) ||
                    document.querySelector(`[name="${inputId}"]`);
            }

            // Se não tiver 'for', procura o input dentro da label
            return label.querySelector('input, select, textarea');
        }

        // Fallback: procura por name, placeholder ou id
        const searchTerms = [
            `[name*="${labelText}"]`,
            `[placeholder*="${labelText}"]`,
            `#${labelText.replace(/\s+/g, '')}`,
            `[name="${labelText.replace(/\s+/g, '_').toLowerCase()}"]`
        ];

        for (const term of searchTerms) {
            const element = document.querySelector(term);
            if (element) return element;
        }

        return null;
    }

    // Função para formatar CNPJ
    function formatCNPJ(cnpj) {
        if (!cnpj || cnpj.length !== 14) return cnpj;
        return cnpj.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, '$1.$2.$3/$4-$5');
    }

    // Função para configurar paginação
    function setupPagination(totalItems) {
        const totalPages = Math.ceil(totalItems / itemsPerPage);
        const paginationDiv = document.getElementById('paginationControls');

        if (totalPages <= 1) {
            paginationDiv.innerHTML = '';
            return;
        }

        paginationDiv.innerHTML = `
            <div class="flex-1 flex items-center justify-between">
                <div>
                    <button id="prevPage" class="relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 ${currentPage === 1 ? 'opacity-50 cursor-not-allowed' : ''}">
                        Anterior
                    </button>
                </div>
                <div class="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
                    <div>
                        <p class="text-sm text-gray-700">
                            Página <span class="font-medium">${currentPage}</span> de <span class="font-medium">${totalPages}</span>
                        </p>
                    </div>
                </div>
                <div>
                    <button id="nextPage" class="relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 ${currentPage === totalPages ? 'opacity-50 cursor-not-allowed' : ''}">
                        Próxima
                    </button>
                </div>
            </div>
        `;

        document.getElementById('prevPage').addEventListener('click', () => {
            if (currentPage > 1) {
                currentPage--;
                renderTable(currentData, currentType);
                setupPagination(totalItems);
            }
        });

        document.getElementById('nextPage').addEventListener('click', () => {
            if (currentPage < totalPages) {
                currentPage++;
                renderTable(currentData, currentType);
                setupPagination(totalItems);
            }
        });
    }

    // Função para configurar busca
    function setupSearch() {
        const searchInput = document.getElementById('searchInput');
        searchInput.value = '';
        searchInput.addEventListener('input', (e) => {
            const searchTerm = e.target.value.toLowerCase();
            if (searchTerm === '') {
                renderTable(currentData, currentType);
                setupPagination(currentData.length);
                return;
            }

            const filteredData = currentData.filter(item => {
                return API_CONFIG[currentType].campos.some(campo => {
                    const value = String(item[campo] || '').toLowerCase();
                    return value.includes(searchTerm);
                });
            });

            currentPage = 1;
            renderTable(filteredData, currentType);
            setupPagination(filteredData.length);
        });
    }

    // Funções auxiliares para mostrar/ocultar elementos
    function showLoading() {
        document.getElementById('loadingIndicator').classList.remove('hidden');
    }

    function hideLoading() {
        document.getElementById('loadingIndicator').classList.add('hidden');
    }

    function showContent() {
        document.getElementById('modalContent').classList.remove('hidden');
    }

    function hideContent() {
        document.getElementById('modalContent').classList.add('hidden');
    }

    function showError(message) {
        const errorDiv = document.getElementById('errorMessage');
        errorDiv.textContent = message;
        errorDiv.classList.remove('hidden');
        hideLoading();
    }

    function hideError() {
        document.getElementById('errorMessage').classList.add('hidden');
    }

    window.preencherFormulario = function (itemData) {
        // Mapeamento de cargos
        const cargos = {
            1: "Administrador",
            2: "Aluno",
            3: "Responsável",
            0: "Não definido"
        };

        // Preenche campos básicos
        document.getElementById('codigoUsuario').value = itemData.id || '';
        document.getElementById('nomeUsuario').value = itemData.nome || '';
        document.getElementById('emailUsuario').value = itemData.email || '';

        // Preenche e gerencia o campo Tipo de Usuário
        const selectTipoUsuario = document.getElementById('tipoUsuario');
        const cargoDisplay = document.getElementById('cargoAtualDisplay');
        const cargoTexto = document.getElementById('cargoAtualTexto');

        // Define o valor do select (para envio)
        selectTipoUsuario.value = itemData.cargo || '';

        // Exibe o texto formatado (para visualização)
        cargoTexto.textContent = cargos[itemData.cargo] || "Desconhecido";
        cargoDisplay.classList.remove('hidden');

        // Preenche empresa (se existir)
        if (selectEmpresa) {
            selectEmpresa.value = itemData.empresaId || '';
        }

        // Limpa campos de senha (segurança)
        document.getElementById('senhaUsuario').value = '';
        document.getElementById('confirmarSenhaUsuario').value = '';

        // Atualiza título
        const titulo = document.querySelector('h2');
        if (titulo) {
            titulo.textContent = 'Editando Usuário: ' + (itemData.nome || '');
        }

        // Dispara eventos para atualizar qualquer validação
        ['tipoUsuario', 'empresa'].forEach(id => {
            const element = document.getElementById(id);
            if (element) {
                element.dispatchEvent(new Event('change'));
            }
        });
    };

});