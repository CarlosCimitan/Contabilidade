﻿using ContabilidadeApi.Data;
using ContabilidadeApi.Dto;
using ContabilidadeApi.Models;
using ContabilidadeApi.Services.SenhaService;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadeApi.Services.UsuarioServices
{
    public class UsuarioService : IUsuario
    {
        private readonly AppDbContext _context;
        private readonly ISenha _senha;

        public UsuarioService(AppDbContext context, ISenha senha)
        {
            _context = context;
            _senha = senha;
        }

        public async Task<ResponseModel<String>> CriarUsuario(CriarUsuarioDto dto)
        {
            ResponseModel<string> resposta = new ResponseModel<string>();

            try
            {
                var usuarioExiste = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);

                if (usuarioExiste != null)
                {
                    resposta.Mensagem = "Usuário já existe.";
                    return resposta;
                }

                _senha.CriarSenhaHash(dto.Senha, out byte[] hash, out byte[] salt);

                Usuario usuario = new Usuario
                {
                    Nome = dto.Nome,
                    Cargo = dto.Cargo,
                    Email = dto.Email,
                    SenhaHash = hash,
                    SenhaSalt = salt,
                    EmpresaId = dto.EmpresaId
                };

                _context.Add(usuario);
                await _context.SaveChangesAsync();

                resposta.Mensagem = "Usuario Criado com Sucesso";


                return resposta;


            }
            catch (Exception ex)
            {
                resposta.Mensagem = "Erro ao criar usuário: " + ex.Message;
                return resposta;
            }
        }

        public async Task<ResponseModel<List<UsuarioDto>>> ListarUsuariosSemEMpresa()
        {
            ResponseModel<List<UsuarioDto>> resposta = new ResponseModel<List<UsuarioDto>>();

            try
            {
                var usuarios = await _context.Usuarios
            .Where(u => u.EmpresaId == null && u.Ativo == true)
            .Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email

            })
            .ToListAsync();
                resposta.Dados = usuarios;
                resposta.Mensagem = "Usuarios encontrados";

                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Mensagem = ex.Message;
                return resposta;
            }
        }

        public async Task<ResponseModel<UsuarioDto>> EditarEmpresaUsuario(UsuarioEmpresaDto dto)
        {
            ResponseModel<UsuarioDto> resposta = new ResponseModel<UsuarioDto>();

            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == dto.Id);

                if (usuario == null)
                {
                    resposta.Mensagem = "Usuário não encontrado.";
                    return resposta;
                }


                usuario.Id = dto.Id;
                usuario.EmpresaId = dto.EmpresaId;


                _context.Update(usuario);
                await _context.SaveChangesAsync();

                resposta.Mensagem = "Usuário editado com sucesso.";
                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Mensagem = "Erro ao editar empresa do usuário: " + ex.Message;
                return resposta;
            }
        }

        public async Task<ResponseModel<UsuarioDto>> EditarUsuario(EditarUsuarioDto dto)
        {
            ResponseModel<UsuarioDto> resposta = new ResponseModel<UsuarioDto>();
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == dto.Id);
                if (usuario == null)
                {
                    resposta.Mensagem = "Usuário não encontrado.";
                    return resposta;
                }

                usuario.Nome = dto.Nome;
                usuario.Cargo = dto.Cargo;
                usuario.Email = dto.Email;
                usuario.EmpresaId = dto.EmpresaId;
                if (!string.IsNullOrEmpty(dto.Senha) && dto.Senha == dto.ConfirmarSenha)
                {
                    _senha.CriarSenhaHash(dto.Senha, out byte[] hash, out byte[] salt);
                    usuario.SenhaHash = hash;
                    usuario.SenhaSalt = salt;
                }
                _context.Update(usuario);
                await _context.SaveChangesAsync();
                resposta.Mensagem = "Usuário editado com sucesso.";
                return resposta;


            }
            catch (Exception ex)
            {
                resposta.Mensagem = "Erro ao editar usuário: " + ex.Message;
                return resposta;
            }
        }

        public async Task<ResponseModel<Usuario>> ExcluirUsuario(int id)
        {
            ResponseModel<Usuario> resposta = new ResponseModel<Usuario>();
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

                if (usuario == null)
                {
                    resposta.Mensagem = "Usuario nao encontrado";
                    return resposta;
                }

                usuario.Ativo = false;

                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();

                resposta.Mensagem = "Usuario excluido com sucesso";
                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Mensagem = ex.Message;
                return resposta;
            }
        }

        public async Task<ResponseModel<List<UsuarioDto>>> BuscarUsuarioPorNome(string nome)
        {
            ResponseModel<List<UsuarioDto>> resposta = new ResponseModel<List<UsuarioDto>>();
            try
            {

                var usuarios = await _context.Usuarios
            .Where(u => u.EmpresaId == null && u.Ativo == true)
            .Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email

            })
            .ToListAsync();
                if (usuarios == null)
                {
                    resposta.Mensagem = "Nenhum usuario com esse nome encontrado";
                    return resposta;
                }

                resposta.Dados = usuarios;
                resposta.Mensagem = "Usuarios econtrados";

                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Mensagem = ex.Message;
                return resposta;
            }
        }

        public async Task<ResponseModel<List<UsuarioDto>>> BuscarUsuarioPorEmpresaId(int id)
        {
            ResponseModel<List<UsuarioDto>> resposta = new ResponseModel<List<UsuarioDto>>();
            try
            {
                var usuarios = await _context.Usuarios
            .Where(e => e.EmpresaId == id && e.Ativo == true)
            .Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                EmpresaId = u.EmpresaId

            })
            .ToListAsync();
                if (usuarios == null)
                {
                    resposta.Mensagem = "Nenhum usuario com essa empresa encontrado";
                    return resposta;
                }

                resposta.Dados = usuarios;
                resposta.Mensagem = "Usuarios econtrados";

                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Mensagem = ex.Message;
                return resposta;
            }
        }

        public async Task<ResponseModel<List<UsuarioDto>>> GetUsuarios()
        {
            ResponseModel<List<UsuarioDto>> resposta = new ResponseModel<List<UsuarioDto>>();
            try
            {
                var usuarios = await _context.Usuarios
                    .Where(u =>  u.Ativo == true)
                    .Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                EmpresaId = u.EmpresaId

            })
            .ToListAsync();
                if (usuarios == null || usuarios.Count == 0)
                {
                    resposta.Mensagem = "Nenhum usuário encontrado.";
                    return resposta;
                }

                resposta.Dados = usuarios;
                resposta.Mensagem = "Usuários encontrados com sucesso.";
                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Mensagem = "Erro ao buscar usuários: " + ex.Message;
                return resposta;
            }
        }
    }
}
