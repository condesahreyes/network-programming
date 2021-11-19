using Grpc.Net.Client;
using IServices;
using LogicaNegocio;
using ServidorAdministrativo;
using System.Threading.Tasks;

namespace Servicios
{ 
    public class UsuarioService : IUsuarioService {

        GrpcChannel canal = GrpcChannel.ForAddress("https://localhost:5001");
        ServicioUsuario.ServicioUsuarioClient userProtoService;

        public UsuarioService()
        {
            this.userProtoService = new ServicioUsuario.ServicioUsuarioClient(canal);
        }

        public async Task<Usuario> ObtenerUsuario(Usuario usuario)
        {
            UsuariosProto usuarios = await userProtoService.ObtenerUsuariosAsync(new MensajeVacio());

            UsuarioProto usuarioRequest = new UsuarioProto { Nombre = usuario.NombreUsuario };
            await userProtoService.AltaUsuarioAsync(usuarioRequest);

            return usuario;
        }
       
        public async Task ActualizarAUsuarioInactivo(string nombreUsuario)
        {
            /*lock (persistencia)
            {
                foreach (Usuario usuario in persistencia.usuarios)
                    if (usuario.NombreUsuario == nombreUsuario)
                        usuario.UsuarioActivo = false;
            }*/
        }

        public async Task ActualizarAUsuarioActivo(string nombreUsuario)
        {
            /*lock (persistencia)
            {
                foreach (Usuario usuario in persistencia.usuarios)
                    if (usuario.NombreUsuario == nombreUsuario)
                        usuario.UsuarioActivo = true;
            }*/
        }
        /*
        public Usuario ObtenerUsuario(Usuario usuario)
        {
            bool noExisteUsuario = NoEsUsuarioExistente(usuario);

            if (noExisteUsuario)
            {
                lock (persistencia)
                {
                    persistencia.usuarios.Add(usuario);
                }
            }

            return DevolverUsuarioExistente(usuario);
        }*/

        public async Task<bool> EliminarUsuario(string nombreUsuario)
        {/*
            lock (persistencia.juegos)
            {
                foreach (Usuario unUsuario in persistencia.usuarios)
                    if (unUsuario.NombreUsuario == nombreUsuario && unUsuario.UsuarioActivo == false)
                    {
                        persistencia.usuarios.Remove(unUsuario);
                        return true;
                    }
                    else if(unUsuario.NombreUsuario == nombreUsuario && unUsuario.UsuarioActivo == true)
                    {
                        Console.WriteLine("Error el usuario no se puede eliminar dado que es un usuario activo");
                        return false;
                    }
            }
            return false;*/
            return false;
        }

        public async Task<bool> ModificarUsuario(string nombreUsuario, string nuevoNombreUsuario)
        {/*
            lock (persistencia.juegos)
            {
                foreach (Usuario unUsuario in persistencia.usuarios)
                    if (unUsuario.NombreUsuario == nombreUsuario && unUsuario.UsuarioActivo == false)
                    {
                        unUsuario.NombreUsuario = nuevoNombreUsuario;
                        return true;
                    }
                    else if (unUsuario.NombreUsuario == nombreUsuario && unUsuario.UsuarioActivo == true)
                    {
                        Console.WriteLine("Error el usuario no se puede modificar dado que es un usuario activo");
                        return false;
                    }
            }
            return false;
            */
            return false;
        }

        public async Task VerListaUsuario()
        {/*
            List<Usuario> usuarios;
            lock (persistencia.usuarios)
            {
                usuarios = persistencia.usuarios;
            }

            if (usuarios.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No se han ingresados usuarios");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (var usuario in usuarios)
                Console.WriteLine(usuario.NombreUsuario) ;   */
        }

        private async Task<bool> NoEsUsuarioExistente(Usuario unUsuario)
        {/*
            List<Usuario> misUsuarios = persistencia.usuarios;
            foreach (var usuario in misUsuarios)
                if (unUsuario.NombreUsuario == usuario.NombreUsuario)
                    return false;

            return true;*/
            return false;
        }

        private async Task<Usuario> DevolverUsuarioExistente(Usuario unUsuario)
        {/*
            List<Usuario> misUsuarios = persistencia.usuarios;
            foreach (var usuario in misUsuarios)
                if (unUsuario.NombreUsuario == usuario.NombreUsuario)
                    return usuario;

            return null;*/
            return null;
        }
    }
}
