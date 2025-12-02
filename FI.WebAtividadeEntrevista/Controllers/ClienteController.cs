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
                // Validar CPF
                if (!bo.ValidarCPF(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json("CPF inválido");
                }

                // Verificar se CPF já existe
                if (bo.VerificarExistencia(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json("CPF já cadastrado");
                }

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

                return Json(model.Id);
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
                // Validar CPF
                if (!bo.ValidarCPF(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json("CPF inválido");
                }

                // Verificar se CPF já existe para outro cliente
                if (bo.VerificarExistencia(model.CPF, model.Id))
                {
                    Response.StatusCode = 400;
                    return Json("CPF já cadastrado para outro cliente");
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