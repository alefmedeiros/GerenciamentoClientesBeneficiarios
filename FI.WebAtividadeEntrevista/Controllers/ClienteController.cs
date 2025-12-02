using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                if (!bo.ValidarCPF(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json("CPF inválido");
                }

                if (bo.VerificarExistencia(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json("CPF já cadastrado");
                }

                if (model.Beneficiarios != null && model.Beneficiarios.Count > 0)
                {
                    BoBeneficiario boBenef = new BoBeneficiario();

                    foreach (var beneficiario in model.Beneficiarios)
                    {
                        if (!boBenef.ValidarCPF(beneficiario.CPF))
                        {
                            Response.StatusCode = 400;
                            return Json("CPF do beneficiário " + beneficiario.Nome + " é inválido");
                        }
                    }

                    var cpfsNormalizados = model.Beneficiarios
                        .Select(b => new string(b.CPF.Where(char.IsDigit).ToArray()))
                        .ToList();

                    var cpfsDuplicados = cpfsNormalizados
                        .GroupBy(cpf => cpf)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key)
                        .ToList();

                    if (cpfsDuplicados.Any())
                    {
                        Response.StatusCode = 400;
                        return Json("CPF duplicado na lista de beneficiários: " + cpfsDuplicados.First());
                    }
                }

                try
                {
                    model.Id = bo.Incluir(new Cliente()
                    {
                        CEP = model.CEP,
                        Cidade = model.Cidade,
                        Email = model.Email,
                        Estado = model.Estado,
                        Logradouro = model.Logradouro,
                        Nacionalidade = model.Nacionalidade,
                        Nome = model.Nome,
                        Sobrenome = model.Sobrenome,
                        Telefone = model.Telefone,
                        CPF = model.CPF
                    });

                    if (model.Beneficiarios != null && model.Beneficiarios.Count > 0)
                    {
                        BoBeneficiario boBenef = new BoBeneficiario();
                        int beneficiariosIncluidos = 0;

                        foreach (var beneficiario in model.Beneficiarios)
                        {
                            try
                            {
                                boBenef.Incluir(new FI.AtividadeEntrevista.DML.Beneficiario()
                                {
                                    CPF = beneficiario.CPF,
                                    Nome = beneficiario.Nome,
                                    IdCliente = model.Id
                                });
                                beneficiariosIncluidos++;
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Erro ao incluir beneficiário {beneficiario.Nome}: {ex.Message}");
                            }
                        }

                        if (beneficiariosIncluidos == 0 && model.Beneficiarios.Count > 0)
                        {
                            Response.StatusCode = 400;
                            return Json("Erro ao incluir beneficiários. Verifique os dados e tente novamente.");
                        }
                    }

                    return Json(model.Id);
                }
                catch (Exception ex)
                {
                    Response.StatusCode = 500;
                    return Json("Erro ao salvar: " + ex.Message);
                }
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                if (!bo.ValidarCPF(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json("CPF inválido");
                }

                if (bo.VerificarExistencia(model.CPF, model.Id))
                {
                    Response.StatusCode = 400;
                    return Json("CPF já cadastrado para outro cliente");
                }

                if (model.Beneficiarios != null && model.Beneficiarios.Count > 0)
                {
                    BoBeneficiario boBenef = new BoBeneficiario();

                    foreach (var beneficiario in model.Beneficiarios)
                    {
                        if (!boBenef.ValidarCPF(beneficiario.CPF))
                        {
                            Response.StatusCode = 400;
                            return Json("CPF do beneficiário " + beneficiario.Nome + " é inválido");
                        }
                    }

                    var cpfsNormalizados = model.Beneficiarios
                        .Select(b => new string(b.CPF.Where(char.IsDigit).ToArray()))
                        .ToList();

                    var cpfsDuplicados = cpfsNormalizados
                        .GroupBy(cpf => cpf)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key)
                        .ToList();

                    if (cpfsDuplicados.Any())
                    {
                        Response.StatusCode = 400;
                        return Json("CPF duplicado na lista de beneficiários: " + cpfsDuplicados.First());
                    }
                }

                bo.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF
                });

                if (model.Beneficiarios != null && model.Beneficiarios.Count > 0)
                {
                    BoBeneficiario boBenef = new BoBeneficiario();

                    List<FI.AtividadeEntrevista.DML.Beneficiario> beneficiariosExistentes = boBenef.ListarPorCliente(model.Id);

                    var idsExistentesNoBanco = new HashSet<long>(beneficiariosExistentes.Select(b => b.Id));

                    var idsRecebidos = model.Beneficiarios
                        .Where(b => b.Id > 0 && idsExistentesNoBanco.Contains(b.Id))
                        .Select(b => b.Id)
                        .ToList();

                    foreach (var benefExistente in beneficiariosExistentes)
                    {
                        if (!idsRecebidos.Contains(benefExistente.Id))
                        {
                            boBenef.Excluir(benefExistente.Id);
                        }
                    }

                    foreach (var beneficiario in model.Beneficiarios)
                    {
                        if (beneficiario.Id > 0 && idsExistentesNoBanco.Contains(beneficiario.Id))
                        {
                            boBenef.Alterar(new FI.AtividadeEntrevista.DML.Beneficiario()
                            {
                                Id = beneficiario.Id,
                                CPF = beneficiario.CPF,
                                Nome = beneficiario.Nome,
                                IdCliente = model.Id
                            });
                        }
                        else
                        {
                            boBenef.Incluir(new FI.AtividadeEntrevista.DML.Beneficiario()
                            {
                                CPF = beneficiario.CPF,
                                Nome = beneficiario.Nome,
                                IdCliente = model.Id
                            });
                        }
                    }
                }
                else
                {
                    BoBeneficiario boBenef = new BoBeneficiario();
                    List<FI.AtividadeEntrevista.DML.Beneficiario> beneficiariosExistentes = boBenef.ListarPorCliente(model.Id);

                    foreach (var benefExistente in beneficiariosExistentes)
                    {
                        boBenef.Excluir(benefExistente.Id);
                    }
                }

                return Json("Cadastro alterado com sucesso");
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    CPF = cliente.CPF
                };
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        #region Beneficiários

        [HttpPost]
        public JsonResult IncluirBeneficiario(BeneficiarioModel model)
        {
            BoBeneficiario bo = new BoBeneficiario();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }

            if (!bo.ValidarCPF(model.CPF))
            {
                Response.StatusCode = 400;
                return Json("CPF inválido");
            }

            if (bo.VerificarExistencia(model.CPF, model.IdCliente))
            {
                Response.StatusCode = 400;
                return Json("CPF já cadastrado para este cliente");
            }

            model.Id = bo.Incluir(new FI.AtividadeEntrevista.DML.Beneficiario()
            {
                CPF = model.CPF,
                Nome = model.Nome,
                IdCliente = model.IdCliente
            });

            return Json(new { success = true, data = model });
        }

        [HttpPost]
        public JsonResult AlterarBeneficiario(BeneficiarioModel model)
        {
            BoBeneficiario bo = new BoBeneficiario();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }

            if (!bo.ValidarCPF(model.CPF))
            {
                Response.StatusCode = 400;
                return Json("CPF inválido");
            }

            if (bo.VerificarExistencia(model.CPF, model.IdCliente, model.Id))
            {
                Response.StatusCode = 400;
                return Json("CPF já cadastrado para outro beneficiário deste cliente");
            }

            bo.Alterar(new FI.AtividadeEntrevista.DML.Beneficiario()
            {
                Id = model.Id,
                CPF = model.CPF,
                Nome = model.Nome,
                IdCliente = model.IdCliente
            });

            return Json(new { success = true, data = model });
        }

        [HttpPost]
        public JsonResult ExcluirBeneficiario(long id)
        {
            BoBeneficiario bo = new BoBeneficiario();
            bo.Excluir(id);
            return Json(new { success = true });
        }

        [HttpGet]
        public JsonResult ListarBeneficiarios(long idCliente)
        {
            BoBeneficiario bo = new BoBeneficiario();
            List<FI.AtividadeEntrevista.DML.Beneficiario> beneficiarios = bo.ListarPorCliente(idCliente);

            var lista = beneficiarios.Select(b => new
            {
                Id = b.Id,
                CPF = b.CPF,
                Nome = b.Nome,
                IdCliente = b.IdCliente
            }).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}