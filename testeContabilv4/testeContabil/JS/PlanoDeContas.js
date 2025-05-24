document.addEventListener("DOMContentLoaded", function() {
    // Definir o botão "Gravar"
    const gravarBtn = document.getElementById("gravar");
    const loadingIndicator = document.getElementById("loadingIndicator"); // Exemplo de feedback visual

    // Quando o botão for clicado
    gravarBtn.addEventListener("click", async function(event) {
        event.preventDefault(); // Evita o envio padrão do formulário

        // Obter os valores dos campos
        const Codigo = document.getElementById("Codigo").value.trim();
        const Mascara = document.getElementById("Mascara").value.trim();
        const Natureza = document.getElementById("Natureza").value;
        const TipoConta = document.getElementById("TipoConta").value;
        const Situacao = document.getElementById("Situacao").value;
        const Descricao = document.getElementById("Descricao").value.trim();

        // Verificar se todos os campos obrigatórios estão preenchidos
        if (!Codigo || !Mascara || !Descricao || !Natureza || !TipoConta || !Situacao) {
            alert("Por favor, preencha todos os campos obrigatórios.");
            return;
        }

        // Definindo os valores para os relatórios
        const livroDiario = document.getElementById("livroDiario").checked;
        const balancoPatrimonial = document.getElementById("balancoPatrimonial").checked;
        const dre = document.getElementById("dre").checked;
        const balancete = document.getElementById("balancete").checked;

        const relatorios = [];
        if (livroDiario) relatorios.push(4);
        if (balancoPatrimonial) relatorios.push(2);
        if (dre) relatorios.push(3);
        if (balancete) relatorios.push(1);

        // Verificar o token de autenticação
        const authToken = localStorage.getItem("authToken");
        if (!authToken) {
            alert("Token de autenticação não encontrado. Por favor, faça login.");
            return;
        }

        // Criar objeto para enviar para a API
        const dadosContaContabil = {
            Codigo: Codigo,
            Mascara: Mascara,
            Natureza: parseInt(Natureza),
            TipoConta: parseInt(TipoConta),
            Situacao: parseInt(Situacao),
            Descricao : Descricao,
           // relatorios: relatorios
        };

        // Enviar os dados via POST para a API C#
        try {
            const response = await fetch("https://localhost:7292/api/ContaContabil/CriarContaContabil", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + authToken // Usando o token para autenticação
                },
                body: JSON.stringify(dadosContaContabil)
            });

            const result = await response.json();
            if (!response.ok) {
                throw new Error(result.mensagem || "Erro ao criar conta contábil");
            }

            console.log("Conta contábil criada com sucesso:", result);
            alert("Conta contábil criada com sucesso!");
        } catch (error) {
            // Esconder o indicador de carregamento em caso de erro
            console.error("Erro ao gravar os dados:", error);
            alert("Houve um erro ao tentar gravar a conta contábil.");
        }
    });
});
