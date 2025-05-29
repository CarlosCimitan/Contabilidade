document.addEventListener('DOMContentLoaded', function () {
    console.log("Página de Cadastro de Empresa carregada.");

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

    const nomeEmpresa = document.getElementById('nomeEmpresa').value.trim();
    const cnpj = document.getElementById('cnpj').value.trim();


    // Validação de campos obrigatórios
    if (!nomeEmpresa || !cnpj) {
        alert('Por favor, preencha todos os campos obrigatórios.');
        return;
    }

    const empresaData = {
        RazaoSocial: nomeEmpresa,
        CNPJ: cnpj,
    };

    // Recupera o token
    const token = localStorage.getItem('authToken');

    try {
        const response = await fetch('https://localhost:7292/api/Empresa/CriarEmpresa', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}` // Aqui passamos o token no cabeçalho
            },
            body: JSON.stringify(empresaData)
        });

        if (response.ok) {
            const result = await response.json();
            alert(result.mensagem || 'Empresa cadastrada com sucesso!');
            // Limpa o formulário, ou redireciona a página, conforme a necessidade
            document.querySelector('form').reset();
        } else {
            let errorData = await response.text();
            try {
                errorData = JSON.parse(errorData);
                alert('Erro ao cadastrar empresa: ' + (errorData.mensagem || 'Erro desconhecido.'));
            } catch {
                alert('Erro ao cadastrar empresa: resposta inválida da API.');
            }
        }
    } catch (error) {
        console.error('Erro ao tentar cadastrar empresa:', error);
        alert('Houve um erro ao tentar cadastrar a empresa.');
    }
});
