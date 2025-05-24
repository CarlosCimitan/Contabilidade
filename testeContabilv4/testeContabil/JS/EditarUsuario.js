document.addEventListener('DOMContentLoaded', async function () {
    console.log("Página de Edição de Usuário carregada.");

    const token = localStorage.getItem('authToken');
    if (!token) {
        alert("Você precisa estar logado para acessar esta página.");
        window.location.href = './login.html';
        return;
    }

    // Carregar usuários cadastrados
    await carregarUsuarios(token);

    // Carregar empresas disponíveis
    await carregarEmpresas(token);

    // Carregar dados do usuário selecionado (mock de exemplo)
    const usuarioSelecionado = JSON.parse(localStorage.getItem('usuarioSelecionado'));
    if (usuarioSelecionado) {
        preencherFormularioUsuario(usuarioSelecionado);
    } else {
        alert("Usuário não encontrado.");
    }
});

// Função para carregar usuários cadastrados
async function carregarUsuarios(token) {
    try {
        const response = await fetch('http://localhost:7292/api/Usuario/GetUsuarios', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            const result = await response.json();
            const usuarios = result.dados || result;

            console.log('Usuários cadastrados:', usuarios);

            // Exibir usuários na tela
            exibirUsuariosNaTela(usuarios);

        } else {
            console.error('Erro ao buscar usuários');
            alert('Erro ao carregar usuários');
        }
    } catch (error) {
        console.error('Erro ao carregar usuários:', error);
        alert('Erro ao tentar carregar usuários');
    }
}

// Função para exibir os usuários na tela
function exibirUsuariosNaTela(usuarios) {
    const usuariosContainer = document.getElementById('usuariosContainer'); // Container onde os usuários serão exibidos

    // Limpa o conteúdo atual
    usuariosContainer.innerHTML = '';

    // Verifique se existem usuários
    if (usuarios.length === 0) {
        usuariosContainer.innerHTML = '<p>Nenhum usuário encontrado.</p>';
    } else {
        // Cria uma lista de usuários
        const listaUsuarios = document.createElement('div');
        listaUsuarios.classList.add('space-y-4');

        usuarios.forEach(usuario => {
            const usuarioDiv = document.createElement('div');
            usuarioDiv.classList.add('p-4', 'bg-gray-100', 'rounded-md', 'shadow-sm');
            usuarioDiv.innerHTML = `
                <p><strong>Nome:</strong> ${usuario.nome}</p>
                <p><strong>Email:</strong> ${usuario.email}</p>
                <p><strong>Tipo:</strong> ${usuario.cargo}</p>
                <button class="editar-btn text-blue-500 hover:underline" data-id="${usuario.id}">Editar</button>
            `;

            listaUsuarios.appendChild(usuarioDiv);
        });

        usuariosContainer.appendChild(listaUsuarios);
    }

    // Adiciona o evento de clique para os botões de editar
    document.querySelectorAll('.editar-btn').forEach(button => {
        button.addEventListener('click', function () {
            const usuarioId = button.getAttribute('data-id');
            
            // Encontre o usuário selecionado
            const usuarioSelecionado = usuarios.find(usuario => usuario.id === usuarioId);
            
            if (usuarioSelecionado) {
                // Salva o objeto do usuário no localStorage para que ele seja acessado na página de edição
                localStorage.setItem('usuarioSelecionado', JSON.stringify(usuarioSelecionado));
                window.location.href = './editarUsuario.html'; // Redireciona para a página de edição
            }
        });
    });
}

// Função para preencher o formulário de edição com os dados do usuário
function preencherFormularioUsuario(usuario) {
    document.getElementById('nomeUsuario').value = usuario.nome || '';
    document.getElementById('emailUsuario').value = usuario.email || '';
    document.getElementById('tipoUsuario').value = usuario.cargo || ''; // Verifique se é 'cargo' ou 'tipo'
    document.getElementById('empresa').value = usuario.empresaId || ''; // Se o usuário não tiver empresa, ficará vazio
}

// Função para carregar empresas disponíveis
async function carregarEmpresas(token) {
    try {
        const response = await fetch('http://localhost:7292/api/Empresa/GetEmpresas', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (response.ok) {
            const result = await response.json();
            const empresas = result.dados || result;

            const selectEmpresa = document.getElementById('empresa');
            selectEmpresa.innerHTML = ''; // Limpa as opções anteriores

            // Cria a opção padrão
            const defaultOption = document.createElement('option');
            defaultOption.value = '';
            defaultOption.textContent = 'Selecione a Empresa';
            selectEmpresa.appendChild(defaultOption);

            // Adiciona as empresas ao select
            empresas.forEach(empresa => {
                const option = document.createElement('option');
                option.value = empresa.id; // A empresa tem um 'id' e 'nome'
                option.textContent = empresa.nome;
                selectEmpresa.appendChild(option);
            });
        } else {
            console.error('Erro ao buscar empresas');
            alert('Erro ao carregar empresas.');
        }
    } catch (error) {
        console.error('Erro ao carregar empresas:', error);
        alert('Erro ao tentar carregar as empresas.');
    }
}
