function gerarRelatorio() {
    const tipoRelatorio = document.getElementById('tipoRelatorio').value;
    const corpoLivroDiario = document.getElementById('corpoLivroDiario');
    const corpoBalancoAtivo = document.getElementById('corpoBalancoAtivo');
    const corpoBalancoPassivo = document.getElementById('corpoBalancoPassivo');
    const corpoDRE = document.getElementById('corpoDRE');
    const corpoCustos = document.getElementById('corpoCustos');
    const corpoDespesas = document.getElementById('corpoDespesas');
    const corpoBalancete = document.getElementById('corpoBalancete');

    // Limpa todas as tabelas
    corpoLivroDiario.innerHTML = '';
    corpoBalancoAtivo.innerHTML = '';
    corpoBalancoPassivo.innerHTML = '';
    corpoDRE.innerHTML = '';
    corpoCustos.innerHTML = '';
    corpoDespesas.innerHTML = '';
    corpoBalancete.innerHTML = '';

    // Chama o backend para pegar os dados do relatório
    const dataInicio = document.getElementById('dataInicio').value;
    const dataFim = document.getElementById('dataFim').value;

    fetch(`http://localhost:5000/api/relatorios/${tipoRelatorio}?dataInicio=${dataInicio}&dataFim=${dataFim}`)
        .then(response => response.json())
        .then(data => {
            if (tipoRelatorio === 'livro_diario') {
                data.forEach((item, index) => {
                    corpoLivroDiario.innerHTML += `
                        <tr>
                            <td class="border-b px-4 py-2">${index + 1}</td>
                            <td class="border-b px-4 py-2">${item.data}</td>
                            <td class="border-b px-4 py-2">${item.historico}</td>
                            <td class="border-b px-4 py-2">${item.debito}</td>
                            <td class="border-b px-4 py-2">${item.credito}</td>
                        </tr>`;
                });
                document.getElementById('relatorioLivroDiario').classList.remove('hidden');
            }
            // Similar para os outros relatórios...
        })
        .catch(error => {
            console.error('Erro ao obter os dados:', error);
        });
}

function baixarRelatorio() {
    const tipoRelatorio = document.getElementById('tipoRelatorio').value;
    const relatorio = document.getElementById(`relatorio${tipoRelatorio.charAt(0).toUpperCase() + tipoRelatorio.slice(1)}`);
    const tabela = relatorio.querySelector('table');
    const wb = XLSX.utils.table_to_book(tabela, { sheet: 'Relatório' });
    XLSX.writeFile(wb, `${tipoRelatorio}.xlsx`);
}

function imprimirRelatorio() {
    const tipoRelatorio = document.getElementById('tipoRelatorio').value;
    const relatorio = document.getElementById(`relatorio${tipoRelatorio.charAt(0).toUpperCase() + tipoRelatorio.slice(1)}`);
    const janela = window.open('', '', 'width=800,height=600');
    janela.document.write(`
        <html>
            <head><title>Relatório Contábil</title></head>
            <body>
                ${relatorio.outerHTML}
            </body>
        </html>
    `);
    janela.document.close();
    janela.print();
}
