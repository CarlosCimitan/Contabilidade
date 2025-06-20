
document.addEventListener('DOMContentLoaded', async function () {
    console.log("Página de Cadastro de Histórico carregada.");
    

    // Autenticação
    const token = localStorage.getItem('authToken');
    if (!token) {
        alert("Você precisa estar logado para acessar esta página.");
        window.location.href = './login.html';
        return;
    }

  document.addEventListener('DOMContentLoaded', () => {

  document.getElementById('btnGravar')?.addEventListener('click', () => {
    const descricao = document.getElementById('descricao').value;
    console.log('Gravando histórico:', descricao);
    alert('Histórico salvo com sucesso!');
  });
  
});
    // Elementos  DOM
    const elements = {
        form: document.getElementById('formHistorico'),
        codigo: document.getElementById('codigoHistorico'),
        descricao: document.getElementById('descricaoHistorico'),
        buttons: {
            gravar: document.getElementById('Gravar'),
            cancelar: document.getElementById('Cancelar'),
            novo: document.getElementById('novo'),
            editar: document.getElementById('editar'),
            listagem: document.getElementById('btnListagem')
        },
        navbar: {
            toggle: document.getElementById('navbar-toggle'),
            menu: document.getElementById('navbar')
        }
    };

    // Estado da Aplicação
    let currentMode = 'new'; 
    let currentId = null;


    if (elements.navbar.toggle && elements.navbar.menu) {
        elements.navbar.toggle.addEventListener('click', () => {
            elements.navbar.menu.classList.toggle('hidden');
        });
    }

    // Carrega ID
    async function carregarProximoId() {
        try {
            const response = await fetch('https://localhost:7292/api/Historico/GetHistoricos', {
                headers: { 'Authorization': `Bearer ${token}` }
            });

            const data = await response.json();
            let proximoId = 1;

            if (data?.dados?.length > 0) {
                proximoId = Math.max(...data.dados.map(hist => hist.id)) + 1;
            }

            elements.codigo.value = proximoId;
            elements.codigo.readOnly = true;
        } catch (error) {
            console.error('Erro ao carregar próximo ID:', error);
            elements.codigo.value = "ERRO";
        }
    }

    // Gravar
    async function gravarHistorico() {
    if (!elements.descricao.value.trim()) {
        alert('Descrição Complementar é obrigatória!');
        return;
    }


    const payload = {
        descricao: elements.descricao.value.trim(),
        empresaId: 0 
    };

    try {
        const url = currentMode === 'edit' && currentId 
            ? `https://localhost:7292/api/Historico/AtualizarHistorico/${currentId}`
            : 'https://localhost:7292/api/Historico/CriarHistorico';

        const method = currentMode === 'edit' ? 'PUT' : 'POST';

        const response = await fetch(url, {
            method,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            alert(`Histórico ${currentMode === 'edit' ? 'atualizado' : 'cadastrado'} com sucesso!`);
            resetForm();
        } else {
            const error = await response.json();
            alert(error.mensagem || 'Erro ao salvar histórico');
        }
    } catch (error) {
        console.error('Erro:', error);
        alert('Erro de rede ao salvar histórico');
    }
}

    function preencherFormulario(historico, mode) {
        currentMode = mode;
        currentId = historico.id;

        elements.codigo.value = historico.id;
        elements.descricao.value = historico.descricao || '';

       
        elements.descricao.readOnly = mode === 'view';
        elements.buttons.gravar.disabled = mode === 'view';
        elements.buttons.editar.disabled = mode === 'edit';
    }

    function resetForm() {
        elements.form.reset();
        currentMode = 'new';
        currentId = null;
        carregarProximoId();
    }
    
    // Excluir
async function excluirHistorico() {
    const id = document.getElementById('codigoHistorico').value;
    
    if (!id || id === "ERRO") {
        alert('Nenhum histórico carregado para excluir');
        return;
    }

    if (!confirm('Tem certeza que deseja excluir este histórico permanentemente?')) {
        return;
    }

    try {
        const response = await fetch(`https://localhost:7292/api/Historico/DeletarHistorico?id=${id}`, {
            method: 'DELETE',
            headers: {
                'accept': '*/*',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`
            }
        });

        if (response.ok) {
            const result = await response.json();
            alert(result.mensagem || 'Histórico excluído com sucesso!');
            resetForm();
        } else {
            throw new Error('Erro ao excluir histórico');
        }
    } catch (error) {
        console.error('Erro na exclusão:', error);
        alert('Falha ao excluir histórico');
    }
}

  // Editar
  document.getElementById('editar')?.addEventListener('click', async function() {
      const id = document.getElementById('codigoHistorico').value;
      const descricao = document.getElementById('descricaoHistorico').value;

      if (!id) {
          alert('Selecione um histórico para editar');
          return;
      }

      try {
          const response = await fetch('https://localhost:7292/api/Historico/EditarHistorico', {
              method: 'PUT',
              headers: {
                  'accept': '*/*',
                  'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
                  'Content-Type': 'application/json'
              },
              body: JSON.stringify({
                  id: parseInt(id),
                  descricao: descricao
              })
          });

          const result = await response.json();
          alert(result.mensagem);
      } catch (error) {
          console.error('Erro ao editar:', error);
          alert('Falha ao editar histórico');
      }
  });
      // --- Event Listeners ---
    elements.buttons.gravar?.addEventListener('click', gravarHistorico);
    elements.buttons.cancelar?.addEventListener('click', resetForm);
    elements.buttons.novo?.addEventListener('click', resetForm);
    document.getElementById('Excluir')?.addEventListener('click', excluirHistorico);
    elements.buttons.editar?.addEventListener('click', () => {
        currentMode = 'edit';
        elements.descricao.readOnly = false;
        elements.buttons.gravar.disabled = false;
    });

    // --- Inicialização ---
    const urlParams = new URLSearchParams(window.location.search);
    const id = urlParams.get('id');
    const mode = urlParams.get('mode');

    if (id) {
        await carregarHistorico(id, mode || 'view');
    } else {
        await carregarProximoId();
    }
});