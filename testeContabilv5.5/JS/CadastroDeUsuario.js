document.addEventListener('DOMContentLoaded', async function () {
    console.log("Página de Cadastro de Usuário carregada.");

        const cargos = {
        1: "Administrador",
        2: "Aluno", 
        3: "Responsável",
        0: "Não definido"
    };

    
    function exibirCargo(valor) {
        return cargos[valor] || "Desconhecido";
    }

    //  Autenticação
    const token = localStorage.getItem('authToken');
    if (!token) {
        alert("Você precisa estar logado para acessar esta página.");
        window.location.href = './login.html';
        return;
    }

    // Elementos  DOM
    const selectEmpresa = document.getElementById('empresa');
    const formUsuario = document.querySelector('form');
    const btnGravar = document.getElementById('Gravar');
    const btnEditar = document.getElementById('editar');
    const btnExcluir = document.getElementById('Excluir');
    const btnNovo = document.getElementById('novo');
    const btnCancelar = document.getElementById('cancelar');
    const btnListagem = document.getElementById('btnListagem');
    const codigoUsuario = document.getElementById('codigoUsuario');


    document.getElementById('navbar-toggle')?.addEventListener('click', () => {
        document.getElementById('navbar').classList.toggle('hidden');
    });

    //Carrega Empresas
    async function carregarEmpresas() {
        try {
            const response = await fetch('https://localhost:7292/api/Empresa/GetEmpresas', {
                headers: { 'Authorization': `Bearer ${token}` }
            });

            if (!response.ok) throw new Error('Erro ao carregar empresas');
            
            const { dados: empresas } = await response.json();
            
            // Limpa e adiciona opção padrão
            selectEmpresa.innerHTML = '';
            selectEmpresa.appendChild(new Option('Selecione uma empresa (opcional)', ''));
            
            // Preenche opções
            empresas.forEach(empresa => {
                selectEmpresa.appendChild(new Option(empresa.razaoSocial, empresa.id));
            });
            
            selectEmpresa.disabled = false;
            
        } catch (error) {
            console.error('Falha ao carregar empresas:', error);
            selectEmpresa.innerHTML = '<option value="">Erro ao carregar empresas</option>';
        }
    }

    // Gravar Usuário
    async function gravarUsuario() {
        const usuario = {
            nome: document.getElementById('nomeUsuario').value.trim(),
            email: document.getElementById('emailUsuario').value.trim(),
            senha: document.getElementById('senhaUsuario').value.trim(),
            confirmarSenha: document.getElementById('confirmarSenhaUsuario').value.trim(),
            cargo: parseInt(document.getElementById('tipoUsuario').value),
            empresaId: selectEmpresa.value ? parseInt(selectEmpresa.value) : null
        };

        // Validação
        if (!usuario.nome || !usuario.email || !usuario.senha || !usuario.confirmarSenha || isNaN(usuario.cargo)) {
            return alert('Preencha todos os campos obrigatórios!');
        }

        if (usuario.senha !== usuario.confirmarSenha) {
            return alert('As senhas não coincidem!');
        }

        try {
            const response = await fetch('https://localhost:7292/api/Usuario/CriarUsuario', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(usuario)
            });

            const data = await response.json();
            
            if (!response.ok) throw new Error(data.mensagem || 'Erro ao cadastrar usuário');
            
            alert('Usuário cadastrado com sucesso!');
            resetForm();
            
        } catch (error) {
            console.error('Erro no cadastro:', error);
            alert(`Falha ao cadastrar: ${error.message}`);
        }
    }

    //Editar Usuário 
async function editarUsuario() {
    const id = document.getElementById('codigoUsuario').value;
    
    if (!id) {
        alert('Nenhum usuário carregado. Selecione um usuário da listagem primeiro.');
        return;
    }

    try {
        // Prepara os dados do formulário
        const usuarioData = {
            id: parseInt(id),
            nome: document.getElementById('nomeUsuario').value.trim(),
            email: document.getElementById('emailUsuario').value.trim(),
            cargo: parseInt(document.getElementById('tipoUsuario').value),
            empresaId: document.getElementById('empresa').value ? parseInt(document.getElementById('empresa').value) : null
        };

        // Verifica e adiciona senha apenas se os campos estiverem preenchidos
        const senha = document.getElementById('senhaUsuario').value.trim();
        const confirmarSenha = document.getElementById('confirmarSenhaUsuario').value.trim();
        
        if (senha || confirmarSenha) {
            if (!senha || !confirmarSenha) {
                throw new Error('Preencha ambos os campos de senha para alterar');
            }
            if (senha !== confirmarSenha) {
                throw new Error('As senhas não coincidem!');
            }
            usuarioData.senha = senha;
            usuarioData.confirmarSenha = confirmarSenha;
        }

        // Validação dos campos obrigatórios
        if (!usuarioData.nome || !usuarioData.email || isNaN(usuarioData.cargo)) {
            throw new Error('Preencha todos os campos obrigatórios!');
        }

        const response = await fetch('https://localhost:7292/api/Usuario/EditarUsuario', {
            method: 'PUT',
            headers: {
                'accept': '*/*',
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(usuarioData)
        });

        // Tratamento da resposta
        let result = {};
        try {
            const text = await response.text();
            result = text ? JSON.parse(text) : {};
        } catch (e) {
            console.warn('Erro ao parsear resposta', e);
        }

        if (!response.ok) {
            throw new Error(result.mensagem || `Erro HTTP ${response.status}`);
        }

        alert(result.mensagem || 'Usuário atualizado com sucesso!');
        resetForm();
        
    } catch (error) {
        console.error('Erro na edição:', error);
        alert(`Falha ao editar: ${error.message}`);
    }
}

    // Excluir Usuário
async function excluirUsuario() {
    const id = document.getElementById('codigoUsuario').value;
    
    if (!id) {
        alert('Selecione um usuário da listagem primeiro.');
        return;
    }

    if (!confirm(`Tem certeza que deseja excluir o usuário ${document.getElementById('nomeUsuario').value}?`)) {
        return;
    }

    try {
        
        const response = await fetch(`https://localhost:7292/api/Usuario/RemoverUsuario?id=${id}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${token}`,
                'accept': '*/*'
            }
        });

  
        if (!response.ok) {
            const errorData = await response.json().catch(() => ({}));
            throw new Error(errorData.mensagem || `Erro HTTP ${response.status}`);
        }

        const result = await response.json();
        alert(result.mensagem || 'Usuário excluído com sucesso!');
        resetForm();
        
    } catch (error) {
        console.error('Erro na exclusão:', error);
        alert(`Falha ao excluir: ${error.message}`);
    }
}
    //Reset Form
    function resetForm() {
        formUsuario.reset();
        codigoUsuario.value = '';
        document.querySelector('h2').textContent = 'Cadastro de Usuário';
    }

    //Event Listeners
    btnGravar?.addEventListener('click', gravarUsuario);
    btnEditar?.addEventListener('click', editarUsuario);
    btnExcluir?.addEventListener('click', excluirUsuario);
    btnNovo?.addEventListener('click', resetForm);
    btnCancelar?.addEventListener('click', resetForm);

    //Modal de Listagem
    window.preencherFormulario = function(itemData) {
        codigoUsuario.value = itemData.id || '';
        document.getElementById('nomeUsuario').value = itemData.nome || '';
        document.getElementById('emailUsuario').value = itemData.email || '';
        document.getElementById('tipoUsuario').value = itemData.cargo || '';
        selectEmpresa.value = itemData.empresaId || '';
        

        
        document.querySelector('h2').textContent = 'Editando Usuário: ' + (itemData.nome || '');
    };

    // Inicialização
    await carregarEmpresas();
});