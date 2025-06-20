document.addEventListener('DOMContentLoaded', async function() {
    // Elementos do DOM
    const form = document.querySelector('form');
    const nomeEmpresa = document.getElementById('nomeEmpresa');
    const cnpj = document.getElementById('cnpj');
    const btnGravar = document.getElementById('Gravar');
    const btnExcluir = document.getElementById('Excluir');
    const btnEditar = document.getElementById('editar');
    const btnListagem = document.getElementById('btnListagem');

    let empresaId = null;

    // 1. Máscara de CNPJ (mantida)
    cnpj.addEventListener('input', function(e) {
        let valor = e.target.value.replace(/\D/g, '');
        if (valor.length > 14) valor = valor.substring(0, 14);
        valor = valor.replace(/^(\d{2})(\d)/, '$1.$2')
                     .replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3')
                     .replace(/\.(\d{3})(\d)/, '.$1/$2')
                     .replace(/(\d{4})(\d)/, '$1-$2');
        e.target.value = valor;
    });

    // 2. Carregar dados para edição (mantido)
    window.carregarEmpresaParaEdicao = function(empresaData) {
        empresaId = empresaData.id;
        nomeEmpresa.value = empresaData.razaoSocial || '';
        cnpj.value = empresaData.cnpj || '';
    };

    // 3. Função para salvar (criar/editar)
 async function criarEmpresa() {
    const dados = {
        razaoSocial: document.getElementById('nomeEmpresa').value.trim(),
        cnpj: document.getElementById('cnpj').value.replace(/\D/g, '')
    };

    try {
        const token = localStorage.getItem('authToken');
        if (!token) {
            alert('Autenticação necessária. Faça login novamente.');
            window.location.href = './login.html';
            return;
        }

        const response = await fetch('https://localhost:7292/api/Empresa/CriarEmpresa', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(dados)
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.mensagem || 'Erro ao criar empresa');
        }

        const result = await response.json();
        alert('Empresa cadastrada com sucesso!');
        form.reset();

    } catch (error) {
        console.error("Erro ao criar:", error);
        alert(`Falha: ${error.message}`);
    }
}
// Editar
async function editarEmpresa() {
    try {
        // 1. Verifica se o elemento existe antes de acessar
        const codigoEmpresaInput = document.getElementById('codigoEmpresa');
        if (!codigoEmpresaInput) {
            throw new Error('Elemento codigoEmpresa não encontrado no formulário');
        }

        const id = codigoEmpresaInput.value;
        if (!id) {
            throw new Error('Nenhuma empresa selecionada para edição');
        }

        // 2. Validação dos elementos do formulário
        const nomeEmpresaInput = document.getElementById('nomeEmpresa');
        const cnpjInput = document.getElementById('cnpj');
        
        if (!nomeEmpresaInput || !cnpjInput) {
            throw new Error('Campos do formulário não encontrados');
        }

        // 3. Prepara os dados com validação
        const dados = {
            id: parseInt(id),
            razaoSocial: nomeEmpresaInput.value.trim(),
            cnpj: cnpjInput.value.replace(/\D/g, '') // Remove formatação do CNPJ
        };

        // 4. Validação dos dados
        if (!dados.razaoSocial || !dados.cnpj) {
            throw new Error('Preencha todos os campos obrigatórios');
        }

        if (dados.cnpj.length !== 14) {
            throw new Error('CNPJ deve ter 14 dígitos');
        }

        // 5. Autenticação
        const token = localStorage.getItem('authToken');
        if (!token) {
            window.location.href = './login.html';
            return;
        }

        // 6. Requisição à API
        const response = await fetch(`https://localhost:7292/api/Empresa/AtualizarEmpresa/${id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(dados)
        });

        // 7. Tratamento da resposta
        if (!response.ok) {
            const errorData = await response.json().catch(() => null);
            throw new Error(errorData?.mensagem || `Erro HTTP ${response.status}`);
        }

        const result = await response.json();
        alert(result.mensagem || 'Empresa atualizada com sucesso!');

    } catch (error) {
        console.error("Erro na edição:", error);
        alert(error.message.includes('Elemento') ? 
              'Erro no formulário. Recarregue a página.' : 
              `Falha: ${error.message}`);
    }
}

    // Excluir
    async function excluirEmpresa() {
        if (!empresaId) {
            alert("Nenhuma empresa selecionada para exclusão.");
            return;
        }

        if (!confirm("Tem certeza que deseja excluir esta empresa?")) {
            return;
        }

        try {
            const token = localStorage.getItem('authToken');
            const response = await fetch(`https://localhost:7292/api/Empresa/ExcluirEmpresa?id=${empresaId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'accept': 'application/json'
                }
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || "Erro ao excluir");
            }

            alert("Empresa excluída com sucesso!");
            form.reset();
            empresaId = null;
        } catch (error) {
            console.error("Erro:", error);
            alert(error.message);
        }
    }

    // Event listeners 
    btnGravar.addEventListener('click', criarEmpresa);
    btnEditar.addEventListener('click', editarEmpresa);
    btnExcluir.addEventListener('click', excluirEmpresa);
    btnListagem.addEventListener('click', () => {
        console.log("Abrindo listagem...");
    });
});