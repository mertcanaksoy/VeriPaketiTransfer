﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace VeriPaketiTransfer
{
    public enum Basliklar : ushort
    {
        Mesaj,
        Resim,
        Nesne,
        Hesap
    }

    public enum Hesaplar : ushort
    {
        Topla, Cikart, Carp, Bol
    }

    [Serializable]
    public class Kisi
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Meslegi { get; set; }
        public int DYili { get; set; }
    }

    public class PaketYazici : BinaryWriter
    {
        MemoryStream _ms;
        BinaryFormatter _bf;

        public PaketYazici() : base()
        {
            _ms = new MemoryStream();
            _bf = new BinaryFormatter();
            OutStream = _ms;
        }

        public byte[] ByteGetir() //Yazılan veriyi byte array olarak geri döndür
        {
            Close();
            return _ms.ToArray();
        }

        public void YazResim(Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, ImageFormat.Png);
            ms.Close();
            byte[] imgdata = ms.ToArray();
            Write(imgdata.Length);
            Write(imgdata);
        }

        public void YazNesne(object obj)
        {
            _bf.Serialize(_ms, obj);
        }
    }

    public class PaketOkuyucu : BinaryReader
    {
        BinaryFormatter _bf;
        public PaketOkuyucu(byte[] data) : base(new MemoryStream(data))
        {
            _bf = new BinaryFormatter();
        }

        public Image GetirResim()
        {
            int boyut = ReadInt32();
            byte[] data = ReadBytes(boyut);
            MemoryStream ms = new MemoryStream(data);
            Image img = Image.FromStream(ms);
            return img;
        }

        public T GetirNesne<T>()
        {
            return (T)_bf.Deserialize(BaseStream);
        }
    }
}
