$(document).ready(function () {
    $('#formCadastro #CPF').on('keyup blur', function () {
        var valor = this.value.replace(/\D/g, '');

        if (valor.length <= 11) {
            valor = valor.replace(/(\d{3})(\d)/, '$1.$2');
            valor = valor.replace(/(\d{3})(\d)/, '$1.$2');
            valor = valor.replace(/(\d{3})(\d{1,2})$/, '$1-$2');
        }

        this.value = valor;
    });

    $('#formCadastro #CPF').on('keypress', function (e) {
        if (this.value.length >= 14 && e.which !== 8) {
            e.preventDefault();
        }
    });

    $('#formCadastro').submit(function (e) {
        e.preventDefault();

        var cpf = $(this).find("#CPF").val();
        if (!validarCPF(cpf)) {
            ModalDialog("Erro de validação", "CPF inválido");
            return false;
        }

        var dadosCliente = {
            "NOME": $(this).find("#Nome").val(),
            "CEP": $(this).find("#CEP").val(),
            "Email": $(this).find("#Email").val(),
            "Sobrenome": $(this).find("#Sobrenome").val(),
            "Nacionalidade": $(this).find("#Nacionalidade").val(),
            "Estado": $(this).find("#Estado").val(),
            "Cidade": $(this).find("#Cidade").val(),
            "Logradouro": $(this).find("#Logradouro").val(),
            "Telefone": $(this).find("#Telefone").val(),
            "CPF": cpf
        };

        if (typeof obterBeneficiariosParaEnvio === 'function') {
            var beneficiarios = obterBeneficiariosParaEnvio();
            console.log('Beneficiários a enviar:', beneficiarios.length, beneficiarios);
            if (beneficiarios.length > 0) {
                dadosCliente.Beneficiarios = beneficiarios;
            }
        }

        $.ajax({
            url: urlPost,
            method: "POST",
            data: JSON.stringify(dadosCliente),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: function (r) {
                if (r.status == 400)
                    ModalDialog("Ocorreu um erro", r.responseJSON);
                else if (r.status == 500)
                    ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
            },
            success: function (r) {
                $("#formCadastro")[0].reset();

                if (typeof beneficiariosTemp !== 'undefined') {
                    beneficiariosTemp = [];
                }

                ModalDialogComRedirect("Sucesso!", "Cadastro efetuado com sucesso!", urlRetorno);
            }
        });
    })
})

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
function ModalDialog(titulo, texto) {
    var random = Math.random().toString().replace('.', '');
    var html = '<div id="' + random + '" class="modal fade">                                                               ' +
        '        <div class="modal-dialog">                                                                                 ' +
        '            <div class="modal-content">                                                                            ' +
        '                <div class="modal-header">                                                                         ' +
        '                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>         ' +
        '                    <h4 class="modal-title">' + titulo + '</h4>                                                    ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-body">                                                                           ' +
        '                    <p>' + texto + '</p>                                                                           ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-footer">                                                                         ' +
        '                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>             ' +
        '                                                                                                                   ' +
        '                </div>                                                                                             ' +
        '            </div><!-- /.modal-content -->                                                                         ' +
        '  </div><!-- /.modal-dialog -->                                                                                    ' +
        '</div> <!-- /.modal -->                                                                                        ';
    $('body').append(html);
    $('#' + random).modal('show');
}

function ModalDialogComRedirect(titulo, texto, urlRedirect) {
    var random = Math.random().toString().replace('.', '');
    var btnId = 'btnOk' + random;
    var html = '<div id="' + random + '" class="modal fade" data-backdrop="static" data-keyboard="false">                  ' +
        '        <div class="modal-dialog">                                                                                 ' +
        '            <div class="modal-content">                                                                            ' +
        '                <div class="modal-header">                                                                         ' +
        '                    <h4 class="modal-title">' + titulo + '</h4>                                                    ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-body">                                                                           ' +
        '                    <p>' + texto + '</p>                                                                           ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-footer">                                                                         ' +
        '                    <button type="button" class="btn btn-primary" id="' + btnId + '">OK</button>                   ' +
        '                </div>                                                                                             ' +
        '            </div><!-- /.modal-content -->                                                                         ' +
        '  </div><!-- /.modal-dialog -->                                                                                    ' +
        '</div> <!-- /.modal -->                                                                                        ';

    $('body').append(html);
    $('#' + random).modal('show');

    $('#' + btnId).click(function () {
        window.location.href = urlRedirect;
    });
}