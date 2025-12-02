var beneficiariosTemp = [];
var clienteIdAtual = 0;
var modoEdicao = false; // false = inclusão, true = alteração

$(document).ready(function () {
    $('#BeneficiarioCPF').on('keyup blur', function () {
        var valor = this.value.replace(/\D/g, '');

        if (valor.length <= 11) {
            valor = valor.replace(/(\d{3})(\d)/, '$1.$2');
            valor = valor.replace(/(\d{3})(\d)/, '$1.$2');
            valor = valor.replace(/(\d{3})(\d{1,2})$/, '$1-$2');
        }

        this.value = valor;
    });

    $('#btnBeneficiarios').click(function (e) {
        e.preventDefault();

        if (obj && obj.Id) {
            modoEdicao = true;
            clienteIdAtual = obj.Id;
            carregarBeneficiarios(clienteIdAtual);
        } else {
            modoEdicao = false;
            clienteIdAtual = 0;
            atualizarGridBeneficiarios();
        }

        $('#modalBeneficiarios').modal('show');
    });

    $('#formBeneficiario').submit(function (e) {
        e.preventDefault();

        var cpf = $('#BeneficiarioCPF').val();
        var nome = $('#BeneficiarioNome').val();
        var id = parseInt($('#BeneficiarioId').val());

        if (!validarCPF(cpf)) {
            alert('CPF inválido!');
            return false;
        }

        if (id !== 0) {
            alterarBeneficiarioTemp(id, cpf, nome);
        } else {
            incluirBeneficiarioTemp(cpf, nome);
        }
    });
});

function carregarBeneficiarios(clienteId) {
    $.ajax({
        url: '/Cliente/ListarBeneficiarios',
        method: 'GET',
        data: { idCliente: clienteId },
        success: function (data) {
            beneficiariosTemp = data;
            atualizarGridBeneficiarios();
        },
        error: function (xhr) {
            alert('Erro ao carregar beneficiários');
        }
    });
}

function normalizarCPF(cpf) {
    return cpf.replace(/\D/g, '');
}

// ===== FUNÇÕES TEMPORÁRIAS (Usadas tanto para inclusão quanto alteração) =====
function incluirBeneficiarioTemp(cpf, nome) {
    var cpfNormalizado = normalizarCPF(cpf);

    var cpfExiste = beneficiariosTemp.some(function (b) {
        return normalizarCPF(b.CPF) === cpfNormalizado;
    });

    if (cpfExiste) {
        alert('CPF já cadastrado para este cliente!');
        return;
    }

    var idTemp = beneficiariosTemp.length > 0
        ? Math.min(...beneficiariosTemp.map(b => b.Id || 0)) - 1
        : -1;

    var novoBeneficiario = {
        Id: idTemp,
        CPF: cpf,
        Nome: nome,
        IdCliente: clienteIdAtual || 0
    };

    beneficiariosTemp.push(novoBeneficiario);
    atualizarGridBeneficiarios();
    limparFormBeneficiario();

    if (modoEdicao) {
        alert('Beneficiário adicionado! Clique em "Salvar" no formulário principal para confirmar.');
    } else {
        alert('Beneficiário adicionado temporariamente! Clique em "Salvar" no formulário principal para confirmar.');
    }
}

function alterarBeneficiarioTemp(id, cpf, nome) {
    var cpfNormalizado = normalizarCPF(cpf);

    var cpfExiste = beneficiariosTemp.some(function (b) {
        return normalizarCPF(b.CPF) === cpfNormalizado && b.Id !== id;
    });

    if (cpfExiste) {
        alert('CPF já cadastrado para outro beneficiário deste cliente!');
        return;
    }

    var beneficiario = beneficiariosTemp.find(function (b) {
        return b.Id === id;
    });

    if (beneficiario) {
        beneficiario.CPF = cpf;
        beneficiario.Nome = nome;
        atualizarGridBeneficiarios();
        limparFormBeneficiario();
        alert('Beneficiário alterado! Clique em "Salvar" no formulário principal para confirmar.');
    } else {
        alert('Erro: Beneficiário não encontrado no array temporário!');
    }
}

function excluirBeneficiarioTemp(id) {
    if (!confirm('Deseja realmente excluir este beneficiário?')) {
        return;
    }

    beneficiariosTemp = beneficiariosTemp.filter(function (b) {
        return b.Id !== id;
    });

    atualizarGridBeneficiarios();
    alert('Beneficiário removido! Clique em "Salvar" no formulário principal para confirmar.');
}

// ===== FUNÇÕES COMUNS =====
function editarBeneficiario(id) {
    var beneficiario = beneficiariosTemp.find(function (b) {
        return b.Id === id;
    });

    if (beneficiario) {
        $('#BeneficiarioId').val(beneficiario.Id);
        $('#BeneficiarioCPF').val(beneficiario.CPF);
        $('#BeneficiarioNome').val(beneficiario.Nome);
        $('#btnSalvarBeneficiario').text('Alterar');

        $('#modalBeneficiarios .modal-body').scrollTop(0);
    }
}

function limparFormBeneficiario() {
    $('#BeneficiarioId').val('0');
    $('#BeneficiarioCPF').val('');
    $('#BeneficiarioNome').val('');
    $('#btnSalvarBeneficiario').text('Incluir');
}

function atualizarGridBeneficiarios() {
    var tbody = $('#tbodyBeneficiarios');
    tbody.empty();

    if (beneficiariosTemp.length === 0) {
        tbody.append('<tr><td colspan="3" class="text-center">Nenhum beneficiário cadastrado</td></tr>');
        return;
    }

    beneficiariosTemp.forEach(function (beneficiario) {
        var tr = $('<tr>');
        tr.append('<td>' + beneficiario.CPF + '</td>');
        tr.append('<td>' + beneficiario.Nome + '</td>');

        tr.append('<td>' +
            '<button class="btn btn-xs btn-primary" onclick="editarBeneficiario(' + beneficiario.Id + ')">Alterar</button> ' +
            '<button class="btn btn-xs btn-primary" onclick="excluirBeneficiarioTemp(' + beneficiario.Id + ')">Excluir</button>' +
            '</td>');
        tbody.append(tr);
    });
}

function validarCPF(cpf) {
    cpf = cpf.replace(/[^\d]+/g, '');
    if (cpf.length != 11 || /^(\d)\1{10}$/.test(cpf)) return false;

    var soma = 0;
    for (var i = 0; i < 9; i++) soma += parseInt(cpf.charAt(i)) * (10 - i);
    var resto = 11 - (soma % 11);
    var digito1 = resto >= 10 ? 0 : resto;
    if (digito1 != parseInt(cpf.charAt(9))) return false;

    soma = 0;
    for (var i = 0; i < 10; i++) soma += parseInt(cpf.charAt(i)) * (11 - i);
    resto = 11 - (soma % 11);
    var digito2 = resto >= 10 ? 0 : resto;
    if (digito2 != parseInt(cpf.charAt(10))) return false;

    return true;
}

function obterBeneficiariosParaEnvio() {
    return beneficiariosTemp.map(function (b) {
        return {
            Id: b.Id > 0 ? b.Id : 0,
            CPF: b.CPF,
            Nome: b.Nome,
            IdCliente: clienteIdAtual || 0
        };
    });
}