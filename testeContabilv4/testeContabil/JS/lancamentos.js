document.addEventListener('DOMContentLoaded', function() {
    // Inicializações
    atualizarDiaSemana();
    
    // Eventos atualizados com novos IDs
    document.getElementById('Data').addEventListener('change', atualizarDiaSemana);
    document.querySelectorAll('.adicionarLinha').forEach(btn => {
        btn.addEventListener('click', clonarLinha);
    });
    document.addEventListener('input', atualizarTotais);
    
    // Botões com novos IDs
    document.getElementById('Cancelar').addEventListener('click', limparFormulario);
    document.getElementById('Gravar').addEventListener('click', gravarLancamento);
    document.getElementById('btnListagem').addEventListener('click', listarLancamentos);

    // Menu responsivo (mantido)
    const toggle = document.getElementById('navbar-toggle');
    const navbar = document.getElementById('navbar');
    toggle.addEventListener('click', () => {
        navbar.classList.toggle('hidden');
    });
});

function clonarLinha(event) {
    const tipoAcao = event.target.getAttribute('data-tipoacao');

    // Encontra o botão clicado
    const botao = event.target;

    // A div pai (wrapper) onde a linha e o botão estão inseridos
    const wrapper = botao.parentNode;

    // Encontra a linha de input correspondente ao tipo
    const linhaOriginal = wrapper.querySelector(`div[data-tipoacao="${tipoAcao}"]`);

    // Clona a linha
    const clone = linhaOriginal.cloneNode(true);

    // Limpa os inputs (exceto os que são readonly)
    clone.querySelectorAll('input').forEach(input => {
        if (!input.readOnly) input.value = '';
    });

    // Insere o clone antes do botão
    wrapper.insertBefore(clone, botao);

    atualizarTotais();
}


// Funções de cálculo e validação
function atualizarTotais() {
    let totalCredito = 0;
    let totalDebito = 0;

    document.querySelectorAll('[data-tipoacao="Credito"] .Valor').forEach(input => {
        totalCredito += parseFloat(input.value) || 0;
    });

    document.querySelectorAll('[data-tipoacao="Debito"] .Valor').forEach(input => {
        totalDebito += parseFloat(input.value) || 0;
    });

    document.getElementById('TotalCredito').value = totalCredito.toFixed(2);
    document.getElementById('TotalDebito').value = totalDebito.toFixed(2);
    document.getElementById('Diferenca').value = (totalCredito - totalDebito).toFixed(2);
}

function atualizarDiaSemana() {
    const dias = ["Domingo", "Segunda-Feira", "Terça-Feira", "Quarta-Feira", 
                 "Quinta-Feira", "Sexta-Feira", "Sábado"];
    const data = new Date(document.getElementById('Data').value);
    if (!isNaN(data)) {
        document.getElementById('DiaSemana').value = dias[data.getDay()];
    }
}

function validarLancamento() {
    const diferenca = parseFloat(document.getElementById('Diferenca').value);
    if (diferenca !== 0) {
        alert("A diferença entre débitos e créditos deve ser zero!");
        return false;
    }
    return true;
}

// Função de gravação alinhada com o C#
async function gravarLancamento() {
    if (!validarLancamento()) return;
    
    const lancamento = {
        Zeramento: document.getElementById('Zeramento').checked,
        DescComplementar: document.getElementById('DescComplementar').value,
        DebitosCreditos: []
    };

    

    // Coleta unificada de débitos/créditos
    document.querySelectorAll('[data-tipoacao]').forEach(linha => {
        const tipo = linha.getAttribute('data-tipoacao');
        const inputs = linha.querySelectorAll('input');
        lancamento.DebitosCreditos.push({
            ContaContabilId: parseInt(inputs[0].value),
            Valor: parseFloat(inputs[3].value),
            TipoAcao: tipo === 'Credito' ? 1 : 0, // 1 = Crédito, 0 = Débito
            DescComplementar: inputs[2].value
        });
    });

    try {
        const response = await fetch('http://localhost:5000/api/LancamentoContabil', {
            method: 'POST',
            headers: { 
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            },
            body: JSON.stringify(lancamento)
        });

        const result = await response.json();
        if (!response.ok) throw new Error(result.Mensagem || 'Erro na requisição');

        alert(result.Mensagem);
        if(response.ok) limparFormulario();
        
    } catch (error) {
        console.error('Erro:', error);
        alert(error.message);
    }
}

// Função para limpar formulário
function limparFormulario() {
    document.getElementById('Zeramento').checked = false;
    document.getElementById('DescComplementar').value = '';
    
    document.querySelectorAll('[data-tipoacao]').forEach((linha, index) => {
        if(index > 0) linha.parentNode.remove();
    });
    
    document.querySelectorAll('.ContaContabilId, .Valor').forEach(input => {
        input.value = '';
    });
    
    atualizarTotais();
}

// Funções auxiliares
async function listarLancamentos() {
    try {
        const response = await fetch('http://localhost:5000/api/LancamentoContabil', {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });
        
        if (!response.ok) throw new Error('Erro ao buscar lançamentos');
        
        const lancamentos = await response.json();
        console.log('Listagem:', lancamentos);
        alert(`Total de lançamentos: ${lancamentos.length}`);
        
    } catch (error) {
        console.error('Erro:', error);
        alert("Erro ao listar lançamentos: " + error.message);
    }
}

// Funções não implementadas (mantidas como placeholder)
function novoApartirDeste() {
    console.warn('Funcionalidade não implementada');
}

function editarLancamento() {
    console.warn('Funcionalidade não implementada');
}