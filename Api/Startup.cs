using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.Domain.Domain;
using minimal_api.Domain.DTO;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Enuns;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.ModelViews;
using minimal_api.Infraestructure.Db;
using minimal_api.Infraestructure.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace minimal_api
{
    public class Startup
    {
        private readonly string key;

        private readonly IConfiguration _Configuration;

        public Startup(IConfiguration configuration)
        {
            _Configuration = configuration;
            key = _Configuration?.GetSection("Jwt")?.ToString() ?? "";
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
            services.AddAuthorization();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira o token JWT aqui"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
            });

            services.AddDbContext<DbContexto>(options =>
            {
                var connection = _Configuration.GetConnectionString("DefaultConnection");
                options.UseMySql(connection, ServerVersion.AutoDetect(connection));
            });

            services.AddScoped<IAdministradorServicos, AdministradorServico>();
            services.AddScoped<IVeiculoServicos, VeiculoServicos>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                #region Home
                endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
                #endregion

                #region Administradores
                string GerarTokenJwt(Administrador administrador)
                {
                    if (string.IsNullOrEmpty(key)) return string.Empty;

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var claims = new List<Claim>()
                {
                    new Claim("Email", administrador.Email),
                    new Claim("Perfil", administrador.Perfil.ToString()),
                    new Claim(ClaimTypes.Role, administrador.Perfil.ToString()),
                };

                    var token = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.Now.AddDays(1),
                        signingCredentials: credentials
                    );

                    return new JwtSecurityTokenHandler().WriteToken(token);
                }

                endpoints.MapPost("/administrador/login", ([FromBody] LoginDTO loginDTO, IAdministradorServicos administradorServicos) =>
                {
                    var adm = administradorServicos.Login(loginDTO);
                    if (adm is not null)
                    {
                        string token = GerarTokenJwt(adm);
                        return Results.Ok(new AdministradorLogado
                        {
                            Email = adm.Email,
                            Perfil = adm.Perfil,
                            Token = token
                        });
                    }
                    else
                        return Results.Unauthorized();
                }).AllowAnonymous().WithTags("Administrador");
                endpoints.MapPost("/administrador", ([FromBody] AdministradorDTO? admDTO, IAdministradorServicos administradorServicos) =>
                {
                    var validacao = new ErrorMessage();

                    if (admDTO == null)
                        validacao.Messagens.Add("Deve ser enviado as infos do adm");
                    if (string.IsNullOrWhiteSpace(admDTO?.Email))
                        validacao.Messagens.Add("Email deve ser informado");
                    if (string.IsNullOrWhiteSpace(admDTO?.Senha))
                        validacao.Messagens.Add("Senha deve ser informado");
                    if (admDTO?.Perfil == EPerfil.none || admDTO?.Perfil is null)
                        validacao.Messagens.Add("Perfil deve ser informado");

                    if (validacao.Messagens.Count == 0)
                    {
                        var adm = administradorServicos.Incluir(new Administrador(admDTO));
                        var amv = new AdministradorModelView(adm.Id, adm.Email, adm.Perfil);
                        return Results.Created($"/administrador/{amv.Id}", amv);
                    }
                    else
                        return Results.BadRequest(validacao);
                }).RequireAuthorization(new AuthorizeAttribute { Roles = EPerfil.adm.ToString() }).WithTags("Administrador");
                endpoints.MapGet("/administrador", ([FromQuery] int? pagina, IAdministradorServicos administradorServicos) =>
                {
                    var adms = administradorServicos.Todos(pagina ?? 1);
                    var admModelView = adms.Select(adm =>
                    {
                        return new AdministradorModelView(adm.Id, adm.Email, adm.Perfil);
                    });
                    return Results.Ok(admModelView);
                }).RequireAuthorization(new AuthorizeAttribute { Roles = EPerfil.adm.ToString() }).WithTags("Administrador");
                endpoints.MapGet("/administrador/{id}", ([FromRoute] int id, IAdministradorServicos administradorServicos) =>
                {
                    try
                    {
                        var adm = administradorServicos.BuscaPorId(id);
                        var admModelView = new AdministradorModelView(adm.Id, adm.Email, adm.Perfil);
                        return Results.Ok(admModelView);
                    }
                    catch (Exception e)
                    {
                        var validacao = new ErrorMessage();
                        validacao.Messagens.Add(e.Message);
                        return Results.NotFound(validacao);
                    }
                }).RequireAuthorization(new AuthorizeAttribute { Roles = EPerfil.adm.ToString() }).WithTags("Administrador");
                #endregion

                #region Veiculos
                endpoints.MapPost("/veiculo", ([FromBody] VeiculoDTO? veiculoDTO, IVeiculoServicos veiculoServicos) =>
                {
                    ErrorMessage validacao = ValidacaoVeiculoDTO(veiculoDTO);

                    if (validacao.Messagens.Count == 0)
                    {
                        var veiculo = veiculoServicos.Incluir(new Veiculo(veiculoDTO));
                        return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
                    }
                    else
                    {
                        return Results.BadRequest(validacao);
                    }
                }).RequireAuthorization().WithTags("Veiculos");
                endpoints.MapGet("/veiculo", ([FromQuery] int? pagina, IVeiculoServicos veiculoServicos) =>
                {
                    var veiculos = veiculoServicos.Todos(pagina ?? 1);
                    return Results.Ok(veiculos);
                }).RequireAuthorization().WithTags("Veiculos");
                endpoints.MapGet("/veiculo/{id}", ([FromRoute] int id, IVeiculoServicos veiculoServicos) =>
                {
                    try
                    {
                        var veiculo = veiculoServicos.BuscaPorId(id);
                        return Results.Ok(veiculo);
                    }
                    catch (Exception e)
                    {
                        var validacao = new ErrorMessage();
                        validacao.Messagens.Add(e.Message);
                        return Results.NotFound(validacao);
                    }
                }).RequireAuthorization().WithTags("Veiculos");
                endpoints.MapPut("/veiculo/{id}", ([FromRoute] int id, [FromBody] VeiculoDTO? veiculoDTO, IVeiculoServicos veiculoServicos) =>
                {
                    var validacao = ValidacaoVeiculoDTO(veiculoDTO);
                    ExisteVeiculo(id, veiculoServicos, validacao);

                    if (validacao.Messagens.Count == 0)
                    {
                        var veiculo = new Veiculo(veiculoDTO)
                        {
                            Id = id
                        };
                        var veiculoAtualizado = veiculoServicos.Atualiza(veiculo);
                        return Results.Ok(veiculoAtualizado);
                    }
                    else
                        return Results.BadRequest(validacao);
                }).RequireAuthorization(new AuthorizeAttribute { Roles = EPerfil.adm.ToString() }).WithTags("Veiculos");
                endpoints.MapDelete("/veiculo/{id}", ([FromRoute] int id, IVeiculoServicos veiculoServicos) =>
                {
                    var validacao = new ErrorMessage();
                    ExisteVeiculo(id, veiculoServicos, validacao);

                    if (validacao.Messagens.Count == 0)
                    {
                        veiculoServicos.Apagar(id);
                        return Results.NoContent();
                    }
                    else
                        return Results.NotFound(validacao);
                }).RequireAuthorization(new AuthorizeAttribute { Roles = EPerfil.adm.ToString() }).WithTags("Veiculos");

                static ErrorMessage ValidacaoVeiculoDTO(VeiculoDTO? veiculoDTO)
                {
                    var validacao = new ErrorMessage();

                    if (veiculoDTO == null)
                        validacao.Messagens.Add("Deve ser enviado as infos de um veiculo");
                    if (string.IsNullOrWhiteSpace(veiculoDTO?.Nome))
                        validacao.Messagens.Add("Nome do veiculo deve ser informado");
                    if (string.IsNullOrWhiteSpace(veiculoDTO?.Marca))
                        validacao.Messagens.Add("Marca do veiculo deve ser informado");
                    if (veiculoDTO?.Ano == null || veiculoDTO?.Ano < 1900)
                        validacao.Messagens.Add("Ano do veiculo muito antigo, informe veiculos apartir de 1900");
                    return validacao;
                }

                static void ExisteVeiculo(int id, IVeiculoServicos veiculoServicos, ErrorMessage errorMessage)
                {
                    if (id <= 0)
                        errorMessage.Messagens.Add("Id do veiculo deve ser informado");
                    else
                    {
                        try
                        {
                            veiculoServicos.BuscaPorId(id);
                        }
                        catch (Exception e)
                        {
                            errorMessage.Messagens.Add(e.Message);
                        }
                    }
                }
                #endregion
            });
        }
    }
}