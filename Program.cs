using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Net;
using Newtonsoft.Json;
using Microsoft.DocAsCode.Common;

namespace _301Sales
{


    class Program
    {
        const String FolderMyApplication = "My Student Application";
        const String RemoteJSON = "http://resources.finance.ua/ru/public/currency-cash.json";
        const String LocalJSON = "currency-cash.json";
        // JSON = JavaScript Object Notation

        private static void ReferenceFill(XmlNode ANode, List<ReferenceItem> AList)
        {
            
            // Очищаем список перед загрузкой
            AList.Clear();
            // загружаем все дочерние узлы в в список справочник
            foreach (XmlNode LNodeChild in ANode.ChildNodes)
            {
                // создаём элемент справочника
                ReferenceItem LReferenceItem = new ReferenceItem();
                // проверяем все аттрибуты текущего дочерниего узла
                foreach (XmlAttribute LAttributesNodeChild in LNodeChild.Attributes)
                {
                    if (LAttributesNodeChild.Name.Equals("id"))
                    {
                        LReferenceItem.Id = LAttributesNodeChild.Value;
                    }
                    if (LAttributesNodeChild.Name.Equals("title"))
                    {
                        LReferenceItem.Name = LAttributesNodeChild.Value;
                    }
                }
                // Добавляем созданный объект в список-справочник
                AList.Add(LReferenceItem);
            }
        }
        private static void ReferencePrintJSON(String ACaption, List<ReferenceItem> AList, ConsoleColor AColor)
        {
            String LStrIDSeparator = "\u2550";
            String LStrNameSeparator = "\u2550";
            int LIDLenght = 0;
            int LNameLenght = 0;
            for (int i = 0; i < AList.Count; i++)
            {
                if (AList[i].Id.Length > LIDLenght) LIDLenght = AList[i].Id.Length;
                if (AList[i].Name.Length > LNameLenght) LNameLenght = AList[i].Name.Length;
            }
            if (LIDLenght < 4) { LIDLenght = 3; }
            for (int i = 1; i < LIDLenght; i++) LStrIDSeparator += "\u2550";
            for (int i = 1; i < LNameLenght; i++) LStrNameSeparator += "\u2550";

            Console.ForegroundColor = AColor;
            Console.WriteLine("\u2554\u2550" + LStrIDSeparator + "\u2550\u2550\u2550" + LStrNameSeparator + "\u2550\u2557");
            Console.WriteLine("\u2551 " + ACaption.PadRight(LIDLenght + LNameLenght + 3) + " \u2551");
            Console.WriteLine("\u2560\u2550" + LStrIDSeparator + "\u2550\u2566\u2550" + LStrNameSeparator + "\u2550\u2563");
            Console.WriteLine("\u2551 " + "Id".PadRight(LIDLenght) + " \u2551 " + "Name".PadRight(LNameLenght) + " \u2551");
            Console.WriteLine("\u2560\u2550" + LStrIDSeparator + "\u2550\u256C\u2550" + LStrNameSeparator + "\u2550\u2563");
            foreach (ReferenceItem LItem in AList)
            {
                Console.WriteLine("\u2551 " + LItem.Id.PadRight(LIDLenght) + " \u2551 " + LItem.Name.PadRight(LNameLenght) + " \u2551");
            }
            Console.WriteLine("\u255A\u2550" + LStrIDSeparator + "\u2550\u2569\u2550" + LStrNameSeparator + "\u2550\u255D");
        }


        public static void DownloadFileText(String ARemoteURL, String ALocalPath)
        {
            WebRequest LWRQ = WebRequest.Create(ARemoteURL);
            LWRQ.Credentials = CredentialCache.DefaultCredentials;
            WebResponse LWR = LWRQ.GetResponse();
            Stream LStrm = LWR.GetResponseStream();
            StreamReader LReader = new StreamReader(LStrm);
            String LFileBody = LReader.ReadToEnd();
            LReader.Close();
            File.WriteAllText(ALocalPath, LFileBody);
        }

        static List<ReferenceItem> RefrerenceFromJSON(dynamic AObject)
        {
            List<ReferenceItem> LResult = new List<ReferenceItem>();
            foreach (var LObj in AObject)
            {
                ReferenceItem LItem = new ReferenceItem();
                LItem.Id = LObj.Name;
                LItem.Name = ((String)LObj.Value);
                LResult.Add(LItem);
            }
            return LResult;
        }

        private static ReferenceItem ReferenceGet(List<ReferenceItem> AList, String aId)
        {
            ReferenceItem LResult = null;
            foreach (ReferenceItem LItem in AList)
            {
                if (LItem.Id.Equals(aId))
                {
                    LResult = LItem;
                    break;
                }
            }
            return LResult;
        }

        static List<OrgsItem> OrganizationsFromJSON(dynamic AObject)
        {
            String r = "";
            List<OrgsItem> LOrganizations = new List<OrgsItem>();
            List<ReferenceItem> LValutes = RefrerenceFromJSON(AObject.currencies);
            List<ReferenceItem> LRegions = RefrerenceFromJSON(AObject.regions);
            List<ReferenceItem> LCities = RefrerenceFromJSON(AObject.cities);
            
            foreach (var LObj in AObject.organizations)
            {
                OrgsItem organization = new OrgsItem();
                
                organization.Id = LObj.id;
                organization.Name = LObj.title;
                organization.Address = LObj.address;
                organization.Phone = LObj.phone;
                organization.Region = LObj.regionId;                                        
                organization.Region = LRegions[0].FindById(organization.Region, LRegions).Name;                                        
                organization.City = LObj.cityId;
                organization.City = LCities[0].FindById(organization.City, LCities).Name;
                
                foreach (var carrency in LObj.currencies)
                {
                    CourceItem Lcarrency = new CourceItem();
                    Lcarrency.Currency = ReferenceGet(LValutes, carrency.Name);
                    Lcarrency.Currency.OrganizationName = organization.Name;
                    Lcarrency.Buy = carrency.Value.bid;
                    Lcarrency.Sale = carrency.Value.ask;
                    organization.Currencies.Add(Lcarrency);
                }
                LOrganizations.Add(organization);   
            }
            return LOrganizations;
        }

        static void ReferencePrintCource(String ACaption, List<OrgsItem> organizations, ConsoleColor AColor = ConsoleColor.White)
        {
            String LenghtOrgName = "\u2550";
            String LenghtOrgId  = "\u2550";
            String LenghtOrgAddr = "\u2550";
            String LenghtOrgPhone = "\u2550";
            String LenghtOrgCity = "\u2550";
            String LenghtOrgReg = "\u2550";

            int OrgNameMax = 0;
            int OrgIdMax = 18;
            int OrgAddrMax = 0;
            int OrgPhoneMax = 14;
            int OrgRegMax = 0;
            int OrgCityMax = 0;
            for (int i = 0; i < organizations.Count; i++)if (organizations[i].Name.Length > OrgNameMax) OrgNameMax = organizations[i].Name.Length;
            for (int i = 0; i < organizations.Count; i++)if (organizations[i].Id.Length > OrgIdMax)         OrgIdMax = organizations[i].Id.Length;
            for (int i = 0; i < organizations.Count; i++)if (organizations[i].Address.Length > OrgAddrMax) OrgAddrMax = organizations[i].Address.Length;    
            for (int i = 0; i < organizations.Count; i++)if (organizations[i].Region.Length > OrgRegMax) OrgRegMax = organizations[i].Region.Length;
            for (int i = 0; i < organizations.Count; i++)if (organizations[i].City.Length > OrgCityMax)    OrgCityMax = organizations[i].City.Length;  
            for (int i = 0; i < OrgNameMax; i++)  LenghtOrgName += "\u2550";
            for (int i = 0; i < OrgIdMax;   i++)  LenghtOrgId   += "\u2550";
            for (int i = 0; i < OrgAddrMax; i++)  LenghtOrgAddr += "\u2550"; 
            for (int i = 0; i < OrgRegMax;  i++)  LenghtOrgReg  += "\u2550";           
            for (int i = 0; i < OrgCityMax; i++)  LenghtOrgCity += "\u2550"; 
            for (int i=0;i<OrgPhoneMax;     i++)  LenghtOrgPhone+= "\u2550";

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\u2554\u2550" + LenghtOrgName + "\u2566" + LenghtOrgAddr + "\u2566" + LenghtOrgPhone + "\u2566" + LenghtOrgReg + "\u2550\u2566\u2550" + LenghtOrgCity +"\u2557");
            Console.WriteLine("\u2551 " + "Имя Орг".PadRight(OrgNameMax) + " \u2551" + "Адресс Орг".PadRight(OrgAddrMax) + " \u2551" + "Телефон Орг".PadRight(OrgPhoneMax) + " \u2551 " + "Регион Орг".PadRight(OrgRegMax) + " \u2551 " + "Город Орг".PadRight(OrgCityMax) + " \u2551");
            Console.WriteLine("\u2560\u2550" + LenghtOrgName + "\u256C" + LenghtOrgAddr + "\u256C" + LenghtOrgPhone + "\u256C" + LenghtOrgReg + "\u2550\u256C\u2550" + LenghtOrgCity + "\u2563");
            foreach (OrgsItem LOrg in organizations)
            { 
             
                Console.WriteLine("\u2551 "+ LOrg.Name.PadLeft(OrgNameMax) + " \u2551" + LOrg.Address.PadLeft(OrgAddrMax) + " \u2551" + String.Format("{0," + OrgPhoneMax.ToString() + ":F4}", LOrg.Phone) + " \u2551 " 
                    + LOrg.Region.PadLeft(OrgRegMax) + " \u2551 " + LOrg.City.PadLeft(OrgCityMax) + " \u2551");
            }
            Console.WriteLine("\u255A\u2550" + LenghtOrgName + "\u2569" + LenghtOrgAddr + "\u2569" + LenghtOrgPhone + "\u2569\u2550" + LenghtOrgReg + "\u2569\u2550" + LenghtOrgCity + "\u255D");

            List<CourceItem> LMaxCurrencies = organizations[0].getMaxCurrency(organizations);
            List<CourceItem> LMinCurrencies = organizations[0].getMinCurrency(organizations);
            String LenghtCursed = "\u2550";
            String CurseItemSeporator = "\u2550";
            String ValItemSeporator = "\u2550";
            String ValuteName = "\u2550";
            String ValuteId = "\u2550";

            int OrgMaxLenght = 0;
            int CurseIDMaxLenght = 18;
            int LenghtCurse = 0;
            int NameCurrenci = 0;
            int IdCurrenci = 0;
            int Curselen = 8;
            
            for (int i = 0; i < organizations.Count; i++)if (organizations[i].Id.Length > OrgMaxLenght) OrgMaxLenght = organizations[i].Id.Length;
            for (int i = 0; i < LMaxCurrencies.Count; i++)if (LMaxCurrencies[i].Currency.Name.Length > NameCurrenci) NameCurrenci = LMaxCurrencies[i].Currency.Name.Length;
            for (int i = 0; i < LMaxCurrencies.Count; i++) if (LMaxCurrencies[i].Currency.Id.Length > IdCurrenci)    IdCurrenci = LMaxCurrencies[i].Currency.Id.Length;
            for (int i = 0; i < NameCurrenci;     i++)   ValuteName         += "\u2550";
            for (int i = 1; i < CurseIDMaxLenght; i++)   ValItemSeporator   += "\u2550";
            for (int i = 1; i < Curselen;         i++)   CurseItemSeporator += "\u2550";
            for (int i = 1; i < LenghtCurse;      i++)   LenghtCursed       += "\u2550";
            for (int i = 0; i < IdCurrenci;       i++)   ValuteId           += "\u2550";
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\u2554\u2550" + ValuteId + "\u2566" + ValuteName + "\u2566" + ValItemSeporator + "\u2550\u2550\u2566" + CurseItemSeporator + "\u2550\u2550\u2566" + ValItemSeporator + "\u2550\u2550\u2550\u2566" + CurseItemSeporator + "\u2550\u2557");
            Console.WriteLine("\u2551 " + "Val".PadRight(IdCurrenci) + " \u2551" + "ValuteName".PadRight(NameCurrenci) + " \u2551" + "Organization".PadRight(OrgMaxLenght) + " \u2551 " + "<=Buy".PadRight(Curselen) + " \u2551 " + "Organization".PadRight(OrgMaxLenght) + " \u2551 " + "<=Sale".PadRight(Curselen) + "\u2551");
            Console.WriteLine("\u2560\u2550" + ValuteId + "\u256C" + ValuteName + "\u256C" + ValItemSeporator + "\u2550\u2550\u256C" + CurseItemSeporator + "\u2550\u2550\u256C" + ValItemSeporator + "\u2550\u2550\u2550\u256C" + CurseItemSeporator + "\u2550\u2563");
            for (int i = 0; i < LMinCurrencies.Count; i++)
            {
                CourceItem MaxCurrencie = LMaxCurrencies[i];
                CourceItem MinCurrencie = LMinCurrencies[i];
                Console.WriteLine("\u2551 " + MaxCurrencie.Currency.Id.PadRight(IdCurrenci) + " \u2551 " + MaxCurrencie.Currency.Name.PadRight(NameCurrenci)
               + "\u2551" + MaxCurrencie.Currency.OrganizationName.PadRight(OrgMaxLenght) + " \u2551 " + String.Format("{0," + Curselen.ToString() + ":F4}", MaxCurrencie.Buy) + " \u2551 " + MinCurrencie.Currency.OrganizationName.PadRight(OrgMaxLenght) + " \u2551 " + String.Format("{0," + Curselen.ToString() + ":F4}", MinCurrencie.Sale) + "\u2551");
            }
            Console.WriteLine("\u255A\u2550" + ValuteId + "\u2569" + ValuteName + "\u2569" + ValItemSeporator + "\u2550\u2550\u2569\u2550" 
              + CurseItemSeporator + "\u2550\u2569\u2550" + ValItemSeporator + "\u2550\u2550\u2569\u2550" + CurseItemSeporator + "\u255D");
        }

        static void Main(string[] args)
        {
            // I. установление путей в приложении
            // I.1 получаем путь к папке "Мои Документы"
            String LDefaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            // I.2 устанавливаем текущим путь "Мои Документы"
            Directory.SetCurrentDirectory(LDefaultPath);
            // I.3 проверяем наличие папки FolderMyApplication
            if (!Directory.Exists(FolderMyApplication))
            { // если нет
                Directory.CreateDirectory(FolderMyApplication); // создаём эту папку
            }
            // I.4 получаем путь к папке "Мои Документы" \ FolderMyApplication
            LDefaultPath = Path.Combine(LDefaultPath, FolderMyApplication);
            // устанавливаем этот путь текущим для приложения
            Directory.SetCurrentDirectory(LDefaultPath);
            // 4.1 скачать файл из интернета
            if (!File.Exists(Path.Combine(LDefaultPath, LocalJSON)))
            {
                DownloadFileText(RemoteJSON, Path.Combine(LDefaultPath, LocalJSON));
            }
            // 4.2. загрузить данные из файла
            String LJSON = File.ReadAllText(Path.Combine(LDefaultPath, LocalJSON));
            // 4.3. контрольный вывод в консоль
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("---------------------------\n");
            //Console.WriteLine(LJSON);
            Console.WriteLine("---------------------------\n");
            // 4.5 неизвестный объект - список курса валют
            var LCourceObject = JsonConvert.DeserializeObject<dynamic>(LJSON);
            // 4.7. парсинг справочников
            List<ReferenceItem> LValutes = RefrerenceFromJSON(LCourceObject.currencies);
            List<ReferenceItem> LRegions = RefrerenceFromJSON(LCourceObject.regions);
            List<ReferenceItem> LCities = RefrerenceFromJSON(LCourceObject.cities);
            List<ReferenceItem> LOrgTypes = RefrerenceFromJSON(LCourceObject.orgTypes);
            List<CourceItem> LCources = new List<CourceItem>();
            List<OrgsItem> LOrganizations = OrganizationsFromJSON(LCourceObject);
            //Выводим справочники
            ReferencePrintJSON("Типы орг", LOrgTypes, ConsoleColor.Yellow);
            ReferencePrintJSON("Регионы", LRegions, ConsoleColor.Gray);
            ReferencePrintJSON("Города", LCities, ConsoleColor.Red);
            ReferencePrintJSON("Валюты", LValutes, ConsoleColor.Green);

            foreach (ReferenceItem LVal in LValutes)
            {
                CourceItem LCourceItem = new CourceItem();
                LCourceItem.Currency = LVal;
                LCourceItem.Buy = 0;
                LCourceItem.Sale = 0;

                LCources.Add(LCourceItem);
            }
            
            ReferencePrintCource("Курсы валют:", LOrganizations, ConsoleColor.Magenta);

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
