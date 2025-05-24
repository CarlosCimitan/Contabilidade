document.addEventListener('DOMContentLoaded', function () {
    console.log("Página de Login carregada.");
});

// Captura e envio de dados do formulário de login
document.getElementById('loginForm').addEventListener('submit', async function(event) {
    event.preventDefault();

    // Captura os dados do formulário
    const email = document.getElementById('email').value;
    const senha = document.getElementById('senha').value;

    // Cria o objeto de dados para enviar
    const loginData = {
        email: email,
        senha: senha
    };

    try {
        // Faz a requisição POST para a API de login
        const response = await fetch('https://localhost:7292/api/Auth/Login', { 
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(loginData)
        });

        if (response.ok) {
            const result = await response.json();
            
            // Se o login for bem-sucedido, armazena o token e redireciona para o dashboard
            localStorage.setItem('authToken', result.dados);  // Armazenando o token no localStorage
            alert('Login bem-sucedido!');  // Mensagem de sucesso
            window.location.href = './dashboard.html';  // Caminho relativo para a raiz
        } else {
            // Se o login falhar, exibe uma mensagem de erro
            const errorData = await response.json();  // Captura o erro da resposta
            alert('Erro: ' + (errorData.mensagem || 'Ocorreu um erro desconhecido.'));  // Exibe a mensagem de erro
        }
        
    } catch (error) {
        console.error('Erro ao tentar fazer login:', error);
        alert('Houve um erro ao tentar fazer login. Tente novamente.');
    }
});
