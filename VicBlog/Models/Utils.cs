﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jose;
using System.Text;
using VicBlog.Data;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace VicBlog.Models
{


    public static class Utils
    {


        public static string USER_TOKEN_KEY;
        public static long LOGIN_EXPIRE_SECONDS;
        public const JwsAlgorithm ALGORITHM = JwsAlgorithm.HS256;

        public static long ToUnixUTCTime(this DateTime datetime)
        {
            return (long)((datetime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds);
        }

        public static DateTime ToLocalDateTime(this long utcTimestamp)
        {
            return (new DateTime(1970, 1, 1, 0, 0, 0)).AddMilliseconds(utcTimestamp).ToLocalTime();
        }

        public static User GetUser(string token, BlogContext context)
        {
            TokenModel convertedToken = JWT.Decode<TokenModel>(token, key: Encoding.ASCII.GetBytes(USER_TOKEN_KEY), alg: ALGORITHM);
            if ((convertedToken.LoginTime - DateTime.Now).Seconds >= LOGIN_EXPIRE_SECONDS)
            {
                throw new TokenOutdatedException();
            }
            return context.Users.Find(convertedToken.Username);
        }

        public async static Task<User> GetUserAsync(string token, BlogContext context)
        {
            TokenModel convertedToken = JWT.Decode<TokenModel>(token, key: Encoding.ASCII.GetBytes(USER_TOKEN_KEY), alg: ALGORITHM);
            if ((DateTime.Now - convertedToken.LoginTime).Seconds >= LOGIN_EXPIRE_SECONDS)
            {
                throw new TokenOutdatedException();
            }
            return await context.Users.FindAsync(convertedToken.Username);
        }

        public static string ComputeMD5(this string rawString)
        {
            MD5 md5 = MD5.Create();
            byte[] encrypted = md5.ComputeHash(Encoding.UTF8.GetBytes(rawString));
            string encryptedString = BitConverter.ToString(encrypted).Replace("-", "");
            md5.Dispose();
            return encryptedString;
        }
        public static string GetIPAddress(this HttpRequest request)
        {
            return request.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach( T i in list)
            {
                action(i);
            }
        }

    }

    
}