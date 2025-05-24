document.addEventListener('DOMContentLoaded', function () {
    console.log("Página de Cadastro de Usuário carregada.");

    // Verifica se o usuário está autenticado
    const token = localStorage.getItem('authToken');
    if (!token) {
        alert("Você precisa estar logado para acessar esta página.");
        window.location.href = './login.html';
        return;
    }
});

// Menu responsivo
const toggle = document.getElementById('navbar-toggle');
const navbar = document.getElementById('navbar');
if (toggle && navbar) {
    toggle.addEventListener('click', () => {
        navbar.classList.toggle('hidden');
    });
}

// Botão Gravar
document.getElementById('gravar').addEventListener('click', async function (event) {
    event.preventDefault();

    const nome = document.getElementById('nomeUsuario').value.trim();
    const email = document.getElementById('emailUsuario').value.trim();
    const senha = document.getElementById('senhaUsuario').value.trim();
    const confirmarSenha = document.getElementById('confirmarSenhaUsuario').value.trim();
    const cargo = parseInt(document.getElementById('tipoUsuario').value);

    if (!nome || !email || !senha || !confirmarSenha) {
        alert('Por favor, preencha todos os campos obrigatórios.');
        return;
    }

    if (senha !== confirmarSenha) {
        alert('As senhas não coincidem!');
        return;
    }

    const usuarioData = {
        nome,
        email,
        senha,
        confirmarSenha,
        cargo
    };

    // Recupera o token
    const token = localStorage.getItem('authToken');

    try {
        const response = await fetch('https://localhost:7292/api/Usuario/CriarUsuario', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}` // ← Aqui está a adição
            },
            body: JSON.stringify(usuarioData)
        });

        if (response.ok) {
            const result = await response.json();
            alert(result.mensagem || 'Usuário cadastrado com sucesso!');
        } else {
            let errorData = await response.text();
            try {
                errorData = JSON.parse(errorData);
                alert('Erro ao cadastrar usuário: ' + (errorData.mensagem || 'Erro desconhecido.'));
            } catch {
                alert('Erro ao cadastrar usuário: resposta inválida da API.');
            }
        }
    } catch (error) {
        console.error('Erro ao tentar cadastrar usuário:', error);
        alert('Houve um erro ao tentar cadastrar o usuário.');
    }
});
