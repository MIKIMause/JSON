﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace _301Sales
{
    /// <summary>
    /// Contragent 
    /// Субъект хозяйственной деятельности, с которым взаимодействует предприятие ЗАКАЗЧЕГа нашего ПО
    /// </summary>
    public class Contragent
    {

        private int FId;           // внутренний код контрагента
        private String FName;      // Название компании. 1) или фамилия частного лица
        private String FNN;        // Налоговый номер
        private String FAddress;   // Адрес
        public Contragent()
        {
            FId = 0;
            FName = "";
            FNN = "";
            FAddress = "";
        }
        /// <summary>
        /// Выводит в консоль контрагента в виде строки
        /// </summary>
        public virtual void Print()
        {
            Console.WriteLine(FId.ToString() + ". " + DisplayName + " | " + FNN + " | " + FAddress);
        }
        protected virtual String GetDisplayName()
        {
            return FName;
        }
        protected virtual String GetName()
        {
            return FName;
        }
        protected virtual void SetName(String NewName)
        {
            if (!FName.Equals(NewName))
            {
                FName = NewName;
            }
        }
        // свойства класса
        /// <summary>
        /// Имя контрагента
        /// </summary>
        public String Name
        {
            get => GetName();
            set { SetName(value); }
        }
        /// <summary>
        /// Отображаемое имя контрагента, кторое может быть разным в классах потомках,
        /// отображая для разных типов контрагентов их название в зависимости от типа
        /// </summary>
        public String DisplayName
        {
            get => GetDisplayName();
        }
        /// <summary>
        /// Уникальный идентификатор контрагента
        /// </summary>
        public int Id
        {
            get => FId;
            set { FId = value; }
        }
        /// <summary>
        /// Налоговый номер контрагента
        /// </summary>
        public String NN
        {
            get => FNN;
            set { FNN = value; }
        }
        /// <summary>
        /// Юридический адрес контрагента
        /// </summary>
        public String Address
        {
            get => FAddress;
            set { FAddress = value; }
        }
    }
    /// <summary>
    /// ContragentPeople - контрагент физическое лицо 
    /// потомок от класса <see cref="T:_301Sales.Contragent"/>
    /// </summary>
    public class ContragentPeople : Contragent
    {

        private String FFirstName;
        private String FMiddleName;

        protected override String GetDisplayName()
        {
            return GetName() + " " + FFirstName.Substring(0, 1) + ". " + FMiddleName.Substring(0, 1) + ".";
        }
        public ContragentPeople()
        {
            FFirstName = "";
            FMiddleName = "";
        }
        /// <summary>
        /// Фамилия (имя рода) физического лица
        /// </summary>
        public String LastName
        {
            get => GetName();
            set { SetName(value); }
        }
        /// <summary>
        /// Личное имя физического лица
        /// </summary>
        public String FirstName
        {
            get => FFirstName;
            set { FFirstName = value; }
        }
        /// <summary>
        /// Второе имя (имя отца, отчество, материство) физического лица
        /// </summary>
        public String MiddleName
        {
            get => FMiddleName;
            set { FMiddleName = value; }
        }

    }
    /// <summary>
    /// ContragentJuridical - контрагент юридическое лицо, потомок от класса <see cref="T:_301Sales.Contragent"/>
    /// </summary>
    public class ContragentJuridical : Contragent
    {
        private bool FVATPay;     // признак плательщика Н.Д.С
        private String FVATCode;    // код плательщика Н.Д.С.

        public ContragentJuridical()
        {
            FVATPay = false;
            FVATCode = "";
        }
        /// <summary>
        /// Выводит в консоль контрагента в виде строки
        /// </summary>
        public override void Print()
        {
            String LResult = Id.ToString() + ". " + DisplayName + " | " + NN + " | ";
            if (FVATPay)
            {
                LResult += "VATCode: " + FVATCode + "|";
            }
            LResult += Address;
            Console.WriteLine(LResult);
        }
        public bool VATPay
        {
            get => FVATPay;
            set { FVATPay = value; }
        }

    }

    // 1. класс - элемент универсального справочника
    public class ReferenceItem
    {
        private String FId;
        private String FName;     
        public String OrganizationName;
        public ReferenceItem()
        {
            FId = "";
            FName = "";
            
        }
        public String Id
        {
            get => FId;
            set { FId = value; }
        }
        public String Name
        {
            get => FName;
            set { FName = value; }
        }
        public ReferenceItem FindById(String Id, List<ReferenceItem> ReferenceList) { 
            foreach (ReferenceItem Item in ReferenceList )
            {
                if (Item.Id==Id)
                {
                    return Item;
                }
            }
            return null;
        }
    }

    public class CourceItem
    {
        public ReferenceItem Currency;
        public float Buy;
        public float Sale;
    }

    public class OrgsItem : ReferenceItem
    {
        private String FOldId;
        private String FAddress;
        private String FPhone;
        private String FLink;
        private String FRegionId;
        private String FCityId;
        private List<CourceItem> FCurrencies;
        private List<CourceItem> FMaxCurrencies;
        private List<CourceItem> FMinCurrencies;

        public OrgsItem()
        {
            FCurrencies = new List<CourceItem>();
            MaxCurrencies = new List<CourceItem>();
            MinCurrencies = new List<CourceItem>();
            
        }
        public String OldId
        {
            get => FOldId;
            set { FOldId = value; }
        }
        public String Address
        {
            get => FAddress;
            set { FAddress = value; }
        }
        public String Phone
        {
            get => FPhone;
            set { FPhone = value; }
        }
        public String Link
        {
            get => FLink;
            set { FLink = value; }
        }
        public String City
        {
            get => FCityId;
            set { FCityId = value; }
        }
        public String Region
        {
            get => FRegionId;
            set { FRegionId = value; }
        }

        private bool isCurrencyInList(CourceItem currency, List<CourceItem> totalList)
        {
            foreach (CourceItem item in totalList)
            {
                if (item.Currency.Id.Equals(currency.Currency.Id))
                {
                    return true;
                }

            }

            return false;
        }

        public List<CourceItem> Currencies
        {
            get => FCurrencies;
            set { FCurrencies = value; }
        }
        public List<CourceItem> MaxCurrencies
        {
            get => FMaxCurrencies;
            set { FMaxCurrencies = value; }
        }
        public List<CourceItem> MinCurrencies
        {
            get => FMinCurrencies;
            set { FMinCurrencies = value; }
        }

        public List<CourceItem> getMaxCurrency(List<OrgsItem> organizations)
        {
            foreach (OrgsItem organization in organizations)
            {
                foreach (CourceItem currency in organization.Currencies)
                {
                    if (MaxCurrencies.Count <= 0)
                    {
                        MaxCurrencies.Add(currency);
                    }
                    else
                    {
                        if (isCurrencyInList(currency, MaxCurrencies))
                        {
                            foreach (CourceItem item in MaxCurrencies)
                            {
                                foreach (CourceItem CurrenciesItem in organization.Currencies)
                                {
                                    if (item.Currency.Id.Equals(CurrenciesItem.Currency.Id))
                                    {
                                        if (item.Buy < CurrenciesItem.Buy)
                                        {
                                            item.Buy = CurrenciesItem.Buy;
                                        }

                                    }
                                }
                            }
                        }
                        else
                        {
                            MaxCurrencies.Add(currency);
                        }

                    }
                }
            }

            return MaxCurrencies;
        }
        public List<CourceItem> getMinCurrency(List<OrgsItem> organizations)
        {
            foreach (OrgsItem organization in organizations)
            {
                foreach (CourceItem currency in organization.Currencies)
                {
                    if (MinCurrencies.Count <= 0)
                    {
                        MinCurrencies.Add(currency);
                    }
                    else
                    {
                        if (isCurrencyInList(currency, MinCurrencies))
                        {
                            foreach (CourceItem item in MinCurrencies)
                            {
                                foreach (CourceItem CurrenciesItem in organization.Currencies)
                                {
                                    if (item.Currency.Id.Equals(CurrenciesItem.Currency.Id))
                                    {
                                        if (item.Sale > CurrenciesItem.Sale)
                                        {
                                            item.Sale = CurrenciesItem.Sale;
                                        }

                                    }
                                }
                            }
                        }
                        else
                        {
                            MinCurrencies.Add(currency);
                        }

                    }
                }
            }
            return MinCurrencies;
        }

    }

}

