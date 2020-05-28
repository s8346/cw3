using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cw3.Services;

namespace cw3.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IStudentsDbService service)
        {
            var path = @"Middleware/requestsLog.txt";
            httpContext.Request.EnableBuffering();
            String wiersz = "";
                if (httpContext.Request!= null)
                {
                    string sciezka = httpContext.Request.Path;
                    string querystring = httpContext.Request.QueryString.ToString();
                    string metoda = httpContext.Request.Method.ToString();
                    string bodyStr = "";
                    wiersz += "Metoda: " + metoda + " ścieżka: " + sciezka;
                    using (StreamReader reader
                     = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                    {
                        bodyStr = await reader.ReadToEndAsync();
                    }
                    if (bodyStr.Length > 0)
                    {
                        wiersz += "\nbody:\n" + bodyStr + "\n";
                }
                else
                {
                    wiersz+= "\nbody: " + "brak" + "\n";
                }

                if (querystring.Length>0)
                {
                    wiersz += " queryString: " + querystring;
                }
                else
                {
                    wiersz += " queryString: brak";
                }

                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(wiersz);
                    file.WriteLine();
                }
            }
            httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            await _next(httpContext);
        }


    }
}
