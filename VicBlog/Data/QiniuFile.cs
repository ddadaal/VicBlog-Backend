using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qiniu.Util;
using Qiniu.IO.Model;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;
using VicBlog.Models;

namespace VicBlog.Data
{
    public class Qiniu
    {
        public static string AccessKey;
        public static string SecretKey;
        public static string BucketName;
        public static int Deadline;
        public static string PostUrl;
        public static string AccessUrl;

        public Auth Auth { get; private set; }

        public Qiniu()
        {
            Auth = new Auth(new Mac(AccessKey, SecretKey));
        }
        public string GetUpdateToken(string fileName)
        {
            PutPolicy putPolicy = new PutPolicy()
            {
                Scope = $"{BucketName}:{fileName}",
            };

            putPolicy.SetExpires(Deadline);

            return Auth.CreateUploadToken(putPolicy.ToJsonString());

        }

        public async Task<QiniuUploadResponse> UploadFileAsync(IFormFile file, string username)
        {
            string fileKey = $"{username}/({DateTime.Now.ToUnixUTCTime()}){file.FileName}";
            using (var client = new HttpClient())
            {
                using (var data = new MultipartFormDataContent())
                {
                    data.Add(new StreamContent(file.OpenReadStream()), "file", fileKey);
                    data.Add(new StringContent(GetUpdateToken(file.FileName)), "token");
                    data.Add(new StringContent(fileKey), "key");
                    var response = await client.PostAsync(PostUrl, data);
                    if (response.IsSuccessStatusCode)
                    {
                        return new QiniuUploadResponse()
                        {
                            AccessUrl = $"{AccessUrl}{fileKey}",
                            Success = true
                        };

                    }
                    else
                    {
                        return new QiniuUploadResponse()
                        {
                            Success = false,
                            Error = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), new { Error = "" }).Error
                        };

                    };
                }
            }


        }
    }
}

