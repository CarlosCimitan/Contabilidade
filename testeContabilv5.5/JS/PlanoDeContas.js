document.addEventListener("DOMContentLoaded", function () {
    // Elementos do DOM
    const editarBtn = document.getElementById("editar");
    const excluirBtn = document.getElementById("Excluir");
    const gravarBtn = document.getElementById("Gravar");
    let contaAtualId = null;

    // Carregar o próximo código
    async function carregarProximoCodigo() {
        const codigoInput = document.getElementById("Codigo");
        if (!codigoInput) {
            console.warn("Campo 'Codigo' não está disponível no DOM para este usuário.");
            return;
        }

        try {
            const authToken = localStorage.getItem("authToken");
            if (!authToken) {
                alert("Autenticação necessária");
                window.location.href = "login.html";
                return;
            }

            const response = await fetch("https://localhost:7292/api/ContaContabil/GetContasContabeis", {
                headers: { "Authorization": `Bearer ${authToken}` }
            });

            if (!response.ok) throw new Error(`Erro HTTP: ${response.status}`);

            const data = await response.json();
            let proximoCodigo = 1;
            const contas = Array.isArray(data) ? data : data?.dados || [];

            if (contas.length > 0) {
                const codigos = contas.map(c => parseInt(c.codigo)).filter(c => !isNaN(c));
                if (codigos.length > 0) {
                    proximoCodigo = Math.max(...codigos) + 1;
                }
            }

            codigoInput.readOnly = false;
            codigoInput.value = proximoCodigo;
            codigoInput.readOnly = true;

        } catch (error) {
            console.error("Erro ao carregar código:", error);
            codigoInput.value = "ERRO";
        }
    }


    // Carregar conta para edição
    window.carregarContaParaEdicao = function (contaData) {
        contaAtualId = contaData.id;
        document.getElementById("Codigo").value = contaData.codigo || '';
        document.getElementById("Mascara").value = contaData.mascara || '';
        document.getElementById("Natureza").value = contaData.natureza || '';
        document.getElementById("TipoConta").value = contaData.tipoConta || '';
        document.getElementById("Grau").value = contaData.grau || '';
        document.getElementById("Descricao").value = contaData.descricao || '';

    };

    // GRAVAR
    gravarBtn.addEventListener("click", async function (event) {
        event.preventDefault();

        const payload = {
            codigo: parseInt(document.getElementById("Codigo").value),
            mascara: document.getElementById("Mascara").value.trim(),
            descricao: document.getElementById("Descricao").value.trim(),
            grau: parseInt(document.getElementById("Grau").value),
            tipoConta: parseInt(document.getElementById("TipoConta").value),
            natureza: parseInt(document.getElementById("Natureza").value),
            tiposRelatorio: getTiposRelatorioSelecionados()
        };


        if (!payload.mascara || !payload.descricao || isNaN(payload.grau) ||
            isNaN(payload.natureza) || isNaN(payload.tipoConta)) {
            alert("Preencha todos os campos obrigatórios!");
            return;
        }

        const authToken = localStorage.getItem("authToken");
        if (!authToken) {
            alert("Autenticação necessária!");
            return;
        }

        try {
            const response = await fetch("https://localhost:7292/api/ContaContabil/CriarContaContabil", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${authToken}`
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) throw new Error(await response.text());

            alert("Conta criada com sucesso!");
            limparFormulario();
        } catch (error) {
            console.error("Erro:", error);
            alert(`Falha ao criar conta: ${error.message}`);
        }
    });

    // EDITAR
    editarBtn.addEventListener("click", async function (event) {
        event.preventDefault();

        if (!contaAtualId) {
            alert("Nenhuma conta selecionada!");
            return;
        }

        const payload = {
            id: contaAtualId,
            codigo: parseInt(document.getElementById("Codigo").value),
            mascara: document.getElementById("Mascara").value.trim(),
            descricao: document.getElementById("Descricao").value.trim(),
            grau: parseInt(document.getElementById("Grau").value),
            tipoConta: parseInt(document.getElementById("TipoConta").value),
            natureza: parseInt(document.getElementById("Natureza").value),
            tiposRelatorio: getTiposRelatorioSelecionados()
        };


        if (!payload.mascara || !payload.descricao || isNaN(payload.grau) ||
            isNaN(payload.natureza) || isNaN(payload.tipoConta)) {
            alert("Preencha todos os campos obrigatórios!");
            return;
        }

        const authToken = localStorage.getItem("authToken");
        if (!authToken) {
            alert("Autenticação necessária!");
            return;
        }

        try {
            const response = await fetch("https://localhost:7292/api/ContaContabil/EditarContaContabil", {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${authToken}`
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) throw new Error(await response.text());

            alert("Conta atualizada com sucesso!");
        } catch (error) {
            console.error("Erro:", error);
            alert(`Falha ao editar conta: ${error.message}`);
        }
    });

    //  EXCLUIR
    document.getElementById("Excluir").addEventListener("click", async function (event) {
        event.preventDefault();

        if (!contaAtualId) {
            alert("Nenhuma conta selecionada para exclusão.");
            return;
        }

        if (!confirm("Tem certeza que deseja excluir esta conta contábil?")) {
            return;
        }

        const authToken = localStorage.getItem("authToken");
        if (!authToken) {
            alert("Autenticação necessária. Faça login novamente.");
            window.location.href = "login.html";
            return;
        }

        try {

            const response = await fetch(`https://localhost:7292/api/ContaContabil/DeletarContaContabil?id=${contaAtualId}`, {
                method: "DELETE",
                headers: {
                    "Authorization": `Bearer ${authToken}`,
                    "accept": "application/json"
                }
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || `Erro ao excluir conta: Status ${response.status}`);
            }

            const result = await response.json();
            alert(result.mensagem || "Conta excluída com sucesso!");
            limparFormulario();

        } catch (error) {
            console.error("Erro na exclusão:", error);
            alert(`Falha ao excluir conta: ${error.message}`);
        }
    });


    document.getElementById("cancelar")?.addEventListener("click", function (event) {
        event.preventDefault();
        limparFormulario();
    });


    document.getElementById("novo")?.addEventListener("click", function (event) {
        event.preventDefault();
        limparFormulario();
    });

    function limparFormulario() {
        document.querySelector("form").reset();
        contaAtualId = null;
        carregarProximoCodigo();
    }

    carregarProximoCodigo();
});