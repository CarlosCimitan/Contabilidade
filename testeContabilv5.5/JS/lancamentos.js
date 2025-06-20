document.addEventListener('DOMContentLoaded', async function () {
    console.log("Página de Lançamento Contábil carregada.");


    // --- Configurações da API ---
    const API_BASE = 'https://localhost:7292/api';
    window.ENDPOINTS = {
        CRIAR_LANCAMENTO: `${API_BASE}/LancamentoContabil/CriarLancamentoContabil`,
        GET_LANCAMENTOS: `${API_BASE}/LancamentoContabil/GetLancamentosContabeis`,
        GET_PLANO_CONTAS_BY_ID: (id) => `${API_BASE}/ContaContabil/GetContasContabeisById?id=${id}`,
        GET_PLANO_CONTAS: `${API_BASE}/ContaContabil/GetContasContabeis`, 
        GET_HISTORICOS: `${API_BASE}/Historico/GetHistoricos`,
        GET_HISTORICO_BY_ID: (id) => `${API_BASE}/Historico/GetHistoricoById?id=${id}`,
        GET_EMPRESAS: `${API_BASE}/Empresa/GetEmpresas`
    };

    // --- Autenticação ---
    const token = localStorage.getItem('authToken');
    if (!token) {
        alert('Você precisa estar logado para acessar esta página.');
        window.location.href = './login.html';
        return; // Sai da função para evitar erros'
    }

    function fillFormFields(itemData) {
        try {
            const clickedButton = document.activeElement;
            if (!clickedButton) {
                console.debug('DEBUG: Nenhum botão ativo encontrado');
                return;
            }

            console.debug('DEBUG: Botão clicado:', clickedButton);
            console.debug('DEBUG: currentType:', currentType);
            console.debug('DEBUG: itemData:', itemData);

            // Verifica se é um botão de listagem de contas (crédito/débito)
            if (clickedButton.id === 'btnListagemPlanos' || clickedButton.hasAttribute('data-conta-type')) {
                const tipoConta = clickedButton.getAttribute('data-conta-type'); // 'credito' ou 'debito'
                console.debug('DEBUG: Tipo de conta identificado:', tipoConta);

                if (!tipoConta) {
                    console.error('DEBUG: Botão não tem data-conta-type definido');
                    return;
                }

                const containerId = `contas${tipoConta.charAt(0).toUpperCase() + tipoConta.slice(1)}`;
                console.debug('DEBUG: Container ID:', containerId);

                const container = document.getElementById(containerId);
                if (!container) {
                    console.error('DEBUG: Container não encontrado:', containerId);
                    return;
                }

                const rows = container.querySelectorAll('.linha-conta');
                const lastRow = rows[rows.length - 1];

                if (lastRow) {
                    const inputId = lastRow.querySelector('[data-conta-input="id"]');
                    if (inputId) {
                        console.debug('DEBUG: Preenchendo conta', tipoConta, 'com ID:', itemData.id);
                        inputId.value = itemData.id || '';
                        // Dispara eventos para buscar automaticamente a descrição e máscara
                        inputId.dispatchEvent(new Event('change', { bubbles: true }));
                        inputId.dispatchEvent(new Event('blur', { bubbles: true }));

                        // Foca no campo de valor
                        const valorClass = tipoConta === 'credito' ? 'valorCredito' : 'valorDebito';
                        const valorInput = lastRow.querySelector(`.${valorClass}`);
                        if (valorInput) {
                            valorInput.focus();
                            console.debug('DEBUG: Focando no campo de valor:', valorClass);
                        }
                    }
                }
                return;
            }

            // Lógica para histórico
            if (currentType === 'historicos') {
                console.debug('DEBUG: Preenchendo campos de histórico');
                safeFill(document, '#historicoIdLancamento', itemData.id);
                safeFill(document, '#descComplementarLancamento', itemData.descricao || '');

                // Atualizar campos relacionados
                const historicoInput = document.getElementById('historicoIdLancamento');
                if (historicoInput) {
                    historicoInput.dispatchEvent(new Event('change', { bubbles: true }));
                    historicoInput.dispatchEvent(new Event('blur', { bubbles: true }));
                }
                return;
            }

            // Lógica para planos
            if (currentType === 'planos' && clickedButton.dataset.contaType) {
                console.debug('DEBUG: Preenchendo campos de plano de contas');
                const row = clickedButton.closest('.flex.space-x-4');
                if (row) {
                    safeFill(row, '[data-conta-input="id"]', itemData.id);
                    safeFill(row, '[data-conta-input="descricao"]', itemData.descricao);
                    safeFill(row, '[data-conta-input="mascara"]', itemData.codigo || '');
                }
                return;
            }

            console.debug('DEBUG: Nenhum caso correspondente encontrado');
        } catch (error) {
            console.error('Erro no fillFormFields:', error);
        }
    }



    function safeFill(context, selector, value) {
        if (!context || !selector) return;
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

    // Auterização
    async function fetchComAuth(url, options = {}) {
        options.headers = options.headers || {};
        options.headers['Content-Type'] = 'application/json';
        options.headers['Authorization'] = `Bearer ${token}`;
        const response = await fetch(url, options);

        // Tratamento de erro centralizado para 401
        if (response.status === 401) {
            alert('Sessão expirada ou não autorizada. Por favor, faça login novamente.');
            window.location.href = './login.html';
            throw new Error('Não autorizado');
        }
        return response;
    }

    //JWT e obter o nome do usuário
    function getUsernameFromToken(jwtToken) {
        if (!jwtToken) return 'Usuário Desconhecido (Token Ausente)';
        try {
            const base64Url = jwtToken.split('.')[1];
            if (!base64Url) {
                console.warn("Token JWT malformado: não possui segunda parte (payload).");
                return 'Usuário Desconhecido (Token Inválido)';
            }
            const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
            const jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            }).join(''));
            const payload = JSON.parse(jsonPayload);
            return payload.Nome || payload.name || payload.unique_name || 'Usuário Logado'; // Prefere 'Nome' do seu token, depois 'name' ou 'unique_name'
        } catch (e) {
            console.error("Erro ao decodificar token para obter nome de usuário:", e);
            return 'Usuário Desconhecido (Erro na Decodificação)';
        }
    }

    // Preencher campos iniciais
    async function preencherCamposIniciais() {
        // Preenche o ID do Lançamento (último + 1)
        const lancamentoIdInput = document.getElementById('Id');
        if (lancamentoIdInput) {
            try {
                const response = await fetchComAuth(window.ENDPOINTS.GET_LANCAMENTOS);
                const data = await response.json();
                let proximoLancamentoId = 1;
                if (data && data.dados && Array.isArray(data.dados) && data.dados.length > 0) {
                    const maiorId = Math.max(...data.dados.map(lanc => lanc.id || lanc.Id));
                    proximoLancamentoId = maiorId + 1;
                }
                lancamentoIdInput.value = proximoLancamentoId;
            } catch (error) {
                console.error('Erro ao carregar lançamentos para determinar o próximo ID:', error);
                if (error.message !== 'Não autorizado') { // Evita alertar duas vezes sobre 401
                    alert('Erro ao carregar o próximo número de lançamento. Pode ser necessário recarregar a página.');
                }
                lancamentoIdInput.value = 'Erro';
            }
        }

        // Preenche Data e Dia da Semana
        const dataInput = document.getElementById('Data');
        if (dataInput) {
            const today = new Date();
            const year = today.getFullYear();
            const month = String(today.getMonth() + 1).padStart(2, '0');
            const day = String(today.getDate()).padStart(2, '0');
            dataInput.value = `${year}-${month}-${day}`;
            atualizarDiaSemana(); // Atualiza o dia da semana com a data atual
        }

        // Preenche o nome do usuário logado
        const usuarioInput = document.getElementById('UsuarioId');
        if (usuarioInput) {
            usuarioInput.value = getUsernameFromToken(token);
        }
    }


    // Inicialização e Eventos
    await preencherCamposIniciais();

    // Verificação de todos os elementos
    const elementos = {
        excluirBtn: document.getElementById('Excluir'),
        dataInput: document.getElementById('Data'),
        addCreditoBtn: document.getElementById('adicionarCredito'),
        addDebitoBtn: document.getElementById('adicionarDebito'),
        cancelarBtn: document.getElementById('Cancelar'),
        gravarBtn: document.getElementById('Gravar'),
        navbarToggle: document.getElementById('navbar-toggle'),
        navbar: document.getElementById('navbar')
    };

    // event listeners com verificação
    if (elementos.excluirBtn) {
        elementos.excluirBtn.addEventListener('click', excluirLancamento);
    } else {
        console.error('Botão Excluir não encontrado - Verifique o HTML');
    }

    if (elementos.dataInput) {
        elementos.dataInput.addEventListener('change', atualizarDiaSemana);
    }

    // Padrão para os demais listeners
    [
        { element: elementos.addCreditoBtn, event: 'click', handler: () => clonarLinha('contasCredito') },
        { element: elementos.addDebitoBtn, event: 'click', handler: () => clonarLinha('contasDebito') },
        { element: elementos.cancelarBtn, event: 'click', handler: limparFormulario },
        { element: elementos.gravarBtn, event: 'click', handler: gravarLancamento }
    ].forEach(({ element, event, handler }) => {
        if (element) {
            element.addEventListener(event, handler);
        }
    });

    // Menu responsivo
    if (elementos.navbarToggle && elementos.navbar) {
        elementos.navbarToggle.addEventListener('click', () => {
            elementos.navbar.classList.toggle('hidden');
        });
    }

    // Evento global
    document.addEventListener('input', atualizarTotais);

    // Inputs das linhas de crédito e débito
    document.querySelectorAll('.codigoConta').forEach(input => {
        input.addEventListener('change', function () {
            buscarContaContabil(this);
        });
        input.addEventListener('blur', function () { // Também no blur
            buscarContaContabil(this);
        });
        input.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                this.blur(); // Simula o blur para disparar a busca
            }
        });
    });

    // Adicione este evento listener para o campo de ID do histórico
    document.getElementById('historicoIdLancamento')?.addEventListener('blur', async function () {
        const id = this.value.trim();
        if (!id) return;

        try {
            const response = await fetchComAuth(window.ENDPOINTS.GET_HISTORICO_BY_ID(id));
            if (!response.ok) throw new Error('Histórico não encontrado');

            const json = await response.json();
            const historico = json.dados;

            if (historico?.descricao) {
                document.getElementById('descComplementarLancamento').value = historico.descricao;
            }
        } catch (error) {
            console.error('Erro ao buscar histórico:', error);
            document.getElementById('descComplementarLancamento').value = '';
        }
    });

    // Preenche descrição do histórico automaticamente
    const historicoIdInput = document.getElementById('HistoricoId');
    if (historicoIdInput) {
        historicoIdInput.addEventListener('change', async function () {
            await buscarHistoricoPorId(this.value.trim());
        });
        historicoIdInput.addEventListener('blur', async function () {
            await buscarHistoricoPorId(this.value.trim());
        });
        historicoIdInput.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                this.blur();
            }
        });
    }


    function preencherContaAutomaticamente(inputElement, contaData) {
        const linha = inputElement.closest('.linha-conta');
        if (!linha) return;

        const fields = {
            id: linha.querySelector('[data-conta-input="id"]'),
            descricao: linha.querySelector('[data-conta-input="descricao"]'),
            mascara: linha.querySelector('[data-conta-input="mascara"]')
        };

        if (fields.id) fields.id.value = contaData.id || '';
        if (fields.descricao) fields.descricao.value = contaData.descricao || '';
        if (fields.mascara) fields.mascara.value = contaData.codigo || '';

        // Foca no campo de valor
        const valorInput = linha.querySelector('.valorCredito, .valorDebito');
        if (valorInput) valorInput.focus();
    }

    async function buscarContaContabil(inputElement) {
        const codigo = inputElement.value.trim();
        const linha = inputElement.closest('.flex.space-x-4');
        // Os índices aqui precisam ser reavaliados se a estrutura HTML mudar
        const mascaraInput = linha.querySelector('.mascaraConta'); // Seleciona pela classe
        const descricaoInput = linha.querySelector('.descricaoConta'); // Seleciona pela classe

        if (!codigo) {
           
            if (mascaraInput) mascaraInput.value = '';
            if (descricaoInput) descricaoInput.value = '';
            return;
        }

        try {
            
            const response = await fetchComAuth(window.ENDPOINTS.GET_PLANO_CONTAS_BY_ID(codigo));

            if (!response.ok) {
                if (response.status === 404) {
                    alert(`Conta Contábil com ID ${codigo} não encontrada.`);
                } else {
                    throw new Error(`Erro HTTP: ${response.status} - ${await response.text()}`);
                }
                inputElement.value = ''; // Limpa o ID se não encontrar
                if (mascaraInput) mascaraInput.value = '';
                if (descricaoInput) descricaoInput.value = '';
                return;
            }

            const json = await response.json();

            const conta = json.dados?.[0]; 

            if (conta && (conta.mascara || conta.descricao)) {
                if (mascaraInput) mascaraInput.value = conta.mascara || '';
                if (descricaoInput) descricaoInput.value = conta.descricao || '';
            } else {
                alert(`Conta Contábil com ID ${codigo} encontrada, mas dados inválidos na resposta da API.`);
                inputElement.value = '';
                if (mascaraInput) mascaraInput.value = '';
                if (descricaoInput) descricaoInput.value = '';
            }
        } catch (err) {
            if (err.message !== 'Não autorizado') { 
                alert(`Erro ao buscar Conta Contábil: ${err.message}`);
            }
            inputElement.value = '';
            if (mascaraInput) mascaraInput.value = '';
            if (descricaoInput) descricaoInput.value = '';
        }
    }

    async function buscarHistoricoPorId(id) {
        const descComplementarInput = document.getElementById('DescComplementar');
        if (!descComplementarInput) {
            console.error("Elemento 'DescComplementar' não encontrado.");
            return;
        }

        if (!id) {
            descComplementarInput.value = ''; // Limpa a descrição se o ID estiver vazio
            return;
        }

        try {
            
            const response = await fetchComAuth(window.ENDPOINTS.GET_HISTORICO_BY_ID(id));

            if (!response.ok) {
                if (response.status === 404) {
                    alert(`Histórico com ID ${id} não encontrado.`);
                } else {
                    throw new Error(`Erro HTTP: ${response.status} - ${await response.text()}`);
                }
                document.getElementById('HistoricoId').value = ''; // Limpa o ID do histórico
                descComplementarInput.value = '';
                return;
            }

            const json = await response.json();
           
            const historico = json.dados;

            if (historico && (historico.descricao || historico.Descricao)) {
                descComplementarInput.value = historico.descricao || historico.Descricao;
            } else {
                alert(`Histórico com ID ${id} encontrado, mas dados inválidos na resposta da API.`);
                document.getElementById('HistoricoId').value = '';
                descComplementarInput.value = '';
            }
        } catch (error) {
            if (error.message !== 'Não autorizado') {
                alert(`Erro ao buscar histórico: ${error.message}`);
            }
            document.getElementById('HistoricoId').value = '';
            descComplementarInput.value = '';
        }
    }

    // limpar campos
    function limparCamposSecundariosDaLinha(linha) {
        const mascaraInput = linha.querySelector('.mascaraConta');
        const descricaoInput = linha.querySelector('.descricaoConta');
        if (mascaraInput) mascaraInput.value = '';
        if (descricaoInput) descricaoInput.value = '';
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
        if (Math.abs(diferenca) > 0.001) { // Usar uma pequena tolerância para floats
            alert("Erro: Débitos e Créditos devem ser iguais!");
            return false;
        }


        // Converta o NodeList para um Array antes de usar .some()
        const debitoInputs = Array.from(document.querySelectorAll('#contasDebito > .flex.space-x-4 input.codigoConta'));
        const creditoInputs = Array.from(document.querySelectorAll('#contasCredito > .flex.space-x-4 input.codigoConta'));

        const hasDebito = debitoInputs.some(input => input.value.trim() !== '');
        const hasCredito = creditoInputs.some(input => input.value.trim() !== '');

        if (!hasDebito || !hasCredito) {
            alert("É necessário informar pelo menos uma conta de débito e uma conta de crédito.");
            return false;
        }

        // Validação adicional: Verifica se cada conta de débito/crédito tem um valor preenchido
        const allDebitoValuesFilled = debitoInputs.every(input => {
            const linha = input.closest('.flex.space-x-4');
            const valorInput = linha.querySelector('.valorDebito');
            return input.value.trim() === '' || (input.value.trim() !== '' && Number(valorInput.value) > 0);
        });

        const allCreditoValuesFilled = creditoInputs.every(input => {
            const linha = input.closest('.flex.space-x-4');
            const valorInput = linha.querySelector('.valorCredito');
            return input.value.trim() === '' || (input.value.trim() !== '' && Number(valorInput.value) > 0);
        });

        if (!allDebitoValuesFilled || !allCreditoValuesFilled) {
            alert("Por favor, preencha os valores para todas as contas de débito e crédito informadas.");
            return false;
        }

        return true;
    }

    async function gravarLancamento() {
        if (!validarLancamento()) return;

        try {
            // 1. Obtenção segura da descrição e ID
            let descHistorico = '';
            const historicoElement = document.getElementById('historicoIdLancamento');
            const historicoId = historicoElement ? Number(historicoElement.value) || 0 : 0;

            if (historicoElement) {
                if (historicoElement.tagName === 'SELECT') {
                    descHistorico = historicoElement.options[historicoElement.selectedIndex]?.text || '';
                } else {
                    descHistorico = historicoElement.value || '';
                }
            }

            const descComplementar = document.getElementById('descComplementarLancamento')?.value || '';
            const descricaoCompleta = descComplementar;

            // 2. Montagem do objeto principal
            const dadosLancamento = {
                data: document.getElementById('Data').value + "T00:00:00.000Z",
                descComplementar: descricaoCompleta,
                historicoId: historicoId, // Variável já definida
                usuarioId: 1,
                debitosCreditos: []
            };


            // 3. Processamento de débitos (com verificação de elementos)
            document.querySelectorAll('#contasDebito > .flex.space-x-4').forEach(linha => {
                const inputs = linha.querySelectorAll('input');
                if (inputs.length >= 4) { // Verifica se tem todos os inputs esperados
                    const contaId = Number(inputs[0].value);
                    const valor = Number(inputs[3].value);

                    if (contaId && valor > 0) {
                        dadosLancamento.debitosCreditos.push({
                            data: dadosLancamento.data,
                            valor: valor,
                            tipoAcao: 1,
                            descComplementar: dadosLancamento.descComplementar,
                            contaContabilId: contaId
                        });
                    }
                }
            });

            // 4. Processamento de créditos (com verificação de elementos)
            document.querySelectorAll('#contasCredito > .flex.space-x-4').forEach(linha => {
                const inputs = linha.querySelectorAll('input');
                if (inputs.length >= 4) {
                    const contaId = Number(inputs[0].value);
                    const valor = Number(inputs[3].value);

                    if (contaId && valor > 0) {
                        dadosLancamento.debitosCreditos.push({
                            data: dadosLancamento.data,
                            valor: valor,
                            tipoAcao: 2,
                            descComplementar: dadosLancamento.descComplementar,
                            contaContabilId: contaId
                        });
                    }
                }
            });

            // 5. Validação final
            if (dadosLancamento.debitosCreditos.length === 0) {
                alert("Adicione pelo menos uma conta de débito e uma de crédito com valores válidos.");
                return;
            }

            // 6. Verificação de equilíbrio contábil
            const totalDebitos = dadosLancamento.debitosCreditos
                .filter(item => item.tipoAcao === 1)
                .reduce((sum, item) => sum + item.valor, 0);

            const totalCreditos = dadosLancamento.debitosCreditos
                .filter(item => item.tipoAcao === 2)
                .reduce((sum, item) => sum + item.valor, 0);

            if (totalDebitos !== totalCreditos) {
                alert(`Erro: O total de débitos (${totalDebitos}) não bate com o total de créditos (${totalCreditos})`);
                return;
            }

            // 7. Envio para API
            const res = await fetchComAuth(window.ENDPOINTS.CRIAR_LANCAMENTO, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(dadosLancamento)
            });

            if (!res.ok) {
                const errorData = await res.json().catch(() => null);
                throw new Error(errorData?.message || `Erro HTTP ${res.status}`);
            }

            alert('Lançamento gravado com sucesso!');
            limparFormulario();

        } catch (error) {
            console.error('Erro no gravarLancamento:', error);
            alert(error.message === 'Não autorizado'
                ? 'Sessão expirada. Faça login novamente.'
                : `Falha ao gravar: ${error.message}`);
        }
    }

    async function excluirLancamento() {
        const id = document.getElementById('Id')?.value;

        if (!id || isNaN(id)) {
            alert('Selecione um lançamento válido para excluir');
            return;
        }

        if (!confirm('Tem certeza que deseja excluir este lançamento permanentemente?')) {
            return;
        }

        try {
            const response = await fetchComAuth(
                `${API_BASE}/LancamentoContabil/DeletarLancamentoContabil/${id}`, // CORREÇÃO AQUI
                {
                    method: 'DELETE',
                    headers: {
                        'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
                        'accept': '*/*' // Adicionado conforme o Swagger
                    }
                }
            );

            if (!response.ok) {
                throw new Error(`Erro HTTP: ${response.status}`);
            }

            const result = await response.json();
            if (result.mensagem) {
                alert(result.mensagem);
            } else {
                alert('Lançamento excluído com sucesso!');
            }

            await limparFormulario();
            await preencherCamposIniciais();
        } catch (error) {
            console.error('Erro na exclusão:', error);
            alert(error.message || 'Falha ao excluir lançamento');
        }
    }




    function clonarLinha(containerId) {
        const container = document.getElementById(containerId);
        const linhas = container.querySelectorAll('.flex.space-x-4');
        const linhaOriginal = linhas[linhas.length - 1]; // Pega a última linha existente
        if (!linhaOriginal) return;

        const novaLinha = linhaOriginal.cloneNode(true);

        // Limpa os valores dos inputs na nova linha
        novaLinha.querySelectorAll('input').forEach(input => {
            if (!input.readOnly) {
                input.value = '';
                // Garante que o input de valor seja numérico e seu valor padrão seja 0 ou vazio para não interferir nos totais
                if (input.type === 'number') input.value = '';
            }
        });

        // Mostra o botão de remoção (que estava hidden na linha original)
        const btnRemover = novaLinha.querySelector('.removerLinha');
        if (btnRemover) {
            btnRemover.classList.remove('hidden');
            btnRemover.addEventListener('click', function () {
                // Só remove se não for a última linha
                if (container.querySelectorAll('.flex.space-x-4').length > 1) {
                    novaLinha.remove();
                    atualizarTotais();
                } else {
                    alert("Você deve manter pelo menos uma linha!");
                }
            });
        }

        // Reconfigura o input de conta para buscar no change/blur
        const inputConta = novaLinha.querySelector('.codigoConta');
        if (inputConta) {
            inputConta.addEventListener('change', function () {
                buscarContaContabil(this);
            });
            inputConta.addEventListener('blur', function () {
                buscarContaContabil(this);
            });
            inputConta.addEventListener('keypress', function (e) {
                if (e.key === 'Enter') {
                    e.preventDefault();
                    this.blur();
                }
            });
        }

        container.appendChild(novaLinha);
        atualizarTotais();
    }

    async function limparFormulario() {
        document.querySelectorAll('input:not([readonly]), textarea').forEach(el => el.value = '');

        // Remove linhas extras, mantendo apenas a primeira
        ['contasCredito', 'contasDebito'].forEach(containerId => {
            const container = document.getElementById(containerId);
            const linhas = container.querySelectorAll('.flex.space-x-4');
            linhas.forEach((linha, idx) => {
                if (idx > 0) linha.remove();
                else { // Limpa a primeira linha
                    linha.querySelectorAll('input').forEach(input => {
                        if (!input.readOnly) input.value = '';
                        if (input.type === 'number') input.value = '';
                    });
                    // Esconde o botão de remover da primeira linha
                    const btnRemover = linha.querySelector('.removerLinha');
                    if (btnRemover) btnRemover.classList.add('hidden');
                }
            });
        });

        await preencherCamposIniciais(); // Recarrega ID do lançamento, data e usuário
        atualizarTotais();
    }

    async function listarGenerico(titulo, endpoint) {
        try {
            // A função `listarGenerico` agora aceita um `endpoint` completo que pode ser a URL de listagem
            // Para as listagens de Contas e Históricos, usamos as URLs de listagem completa que você sugeriu.
            const res = await fetchComAuth(endpoint);
            if (!res.ok) {
                throw new Error(`Erro ao buscar dados de ${titulo}. Status: ${res.status}`);
            }
            const json = await res.json();
            console.log(`Dados de ${titulo}:`, json); // Loga o objeto completo para depuração
            console.table(json.dados || json); // Tenta logar a tabela se houver um array 'dados'
            alert(`${titulo} carregado. Veja o console (F12) para a tabela de dados.`);
        } catch (error) {
            if (error.message !== 'Não autorizado') {
                alert(`Erro ao buscar ${titulo}: ${error.message}`);
            }
        }
    }

    function atualizarDiaSemana() {
        const dataStr = document.getElementById('Data').value;
        if (!dataStr) {
            document.getElementById('DiaSemana').value = '';
            return;
        }
        const data = new Date(dataStr + 'T00:00:00'); // Adiciona T00:00:00 para evitar problemas de fuso horário
        const diaSemana = data.toLocaleDateString('pt-BR', { weekday: 'long' });
        document.getElementById('DiaSemana').value = diaSemana.charAt(0).toUpperCase() + diaSemana.slice(1);
    }




    document.getElementById('novo').addEventListener('click', limparFormulario); // 'Novo' já chama limparFormulario

});