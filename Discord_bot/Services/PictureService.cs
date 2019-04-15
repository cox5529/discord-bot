using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Discord_bot.Services {
    public class PictureService {
        private readonly HttpClient _http;
        private readonly IConfigurationRoot _configuration;

        public PictureService(HttpClient http, IConfigurationRoot configuration) {
            _http = http;
            _configuration = configuration;
        }

        public async Task<Stream> GetCatPictureAsync() {
            var resp = await _http.GetAsync("https://cataas.com/cat");
            return await resp.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> GetLocalImage(string filename) {
            using (var fileStream = new FileStream(_configuration.GetSection("images")[filename], FileMode.Open)) {
                Stream s = new MemoryStream();
                await fileStream.CopyToAsync(s);
                return s;
            }
        }
    }
}