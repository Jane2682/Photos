using System;
using System.Collections.Generic;
using System.Text;
//4.solis
namespace Photos
{
    public static class Literals// klase kas laus izmantot dazadus savienojumus lietotnes vairakas funkcijas
    {
        public const string StorageConnectionString = nameof(StorageConnectionString);//rinda kas tiek
    
        public const string CosmosDBConnection = nameof(CosmosDBConnection);
        // veidojam so ierakstu lai izmantot CosmosDB savienojumu vairakas lietotnes funkcijas
    }
}
