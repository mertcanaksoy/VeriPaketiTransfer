﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VeriPaketiTransfer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            cmbHesap.SelectedIndex = 0;
            btnHesap.Click += btnTiklamalar;
            btnMesaj.Click += btnTiklamalar;
            btnNesne.Click += btnTiklamalar;
            btnResim.Click += btnTiklamalar;
        }

        void btnTiklamalar(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            PaketYazici py = new PaketYazici();

            switch (btn.Name)
            {
                case "btnMesaj":
                    py.Write((ushort)Basliklar.Mesaj);
                    py.Write(txtMesaj.Text);
                    break;
                case "btnResim":
                    using (OpenFileDialog ofd = new OpenFileDialog())
                    {
                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            py.Write((ushort)Basliklar.Resim);
                            Image img = Image.FromFile(ofd.FileName);
                            py.YazResim(img);
                        }
                    }

                    break;
                case "btnNesne":
                    Kisi k = new Kisi();
                    k.Adi = txtAdi.Text;
                    k.Soyadi = txtSoyadi.Text;
                    k.Meslegi = txtMeslegi.Text;
                    if (txtDYili.Text=="")
                    {
                        k.DYili=0;
                    }
                    else
                    {
                        k.DYili = int.Parse(txtDYili.Text);
                    }
                    
                    py.Write((ushort)Basliklar.Nesne);
                    py.YazNesne(k);
                    break;
                case "btnHesap":
                    py.Write((ushort)Basliklar.Hesap);
                    switch (cmbHesap.SelectedIndex)
                    {
                        case 0: //Topla
                            py.Write((ushort)Hesaplar.Topla);
                            break;
                        case 1: //Çıkart
                            py.Write((ushort)Hesaplar.Cikart);
                            break;
                        case 2: //Çarp
                            py.Write((ushort)Hesaplar.Carp);
                            break;
                        case 3: //Böl
                            py.Write((ushort)Hesaplar.Bol);
                            break;
                    }
                    py.Write(int.Parse(txtS1.Text));
                    py.Write(int.Parse(txtS2.Text));
                    break;
            }

            TerminalIslem(py.ByteGetir()); //Paketlenen verileri byteArray olarak terminale gönder
        }

        void TerminalIslem(byte[] veri) //Gelen verileri işleme
        {
            PaketOkuyucu po = new PaketOkuyucu(veri);
            Basliklar baslik = (Basliklar)po.ReadUInt16();

            switch (baslik)
            {
                case Basliklar.Mesaj:
                    MessageBox.Show(string.Format("Gelen mesaj:\r\n{0}", po.ReadString()));
                    break;
                case Basliklar.Resim:
                    pcbResim.Image = po.GetirResim();
                    break;
                case Basliklar.Nesne:
                    Kisi k = (Kisi)po.GetirNesne<Kisi>();
                    MessageBox.Show(string.Format("Gelen Nesne Özellikleri:\r\nAd: {0}\r\nSoyadı: {1}\r\nMesleği: {2}\r\nD.Yılı:{3}", k.Adi, k.Soyadi, k.Meslegi, k.DYili));
                    break;
                case Basliklar.Hesap:
                    Hesaplar hesap = (Hesaplar)po.ReadUInt16();
                    int s1 = po.ReadInt32();
                    int s2 = po.ReadInt32();
                    double s3 = 0;
                    switch (hesap)
                    {
                        case Hesaplar.Topla:
                            s3 = s1 + s2;
                            MessageBox.Show(string.Format("{0} sayısı ile {1} sayısının toplama işlemi sonucu {2} çıkmıştır", s1, s2, s3));
                            break;
                        case Hesaplar.Cikart:
                            s3 = s1 - s2;
                            MessageBox.Show(string.Format("{0} sayısı ile {1} sayısının çıkartma işlemi sonucu {2} çıkmıştır", s1, s2, s3));
                            break;
                        case Hesaplar.Carp:
                            s3 = s1 * s2;
                            MessageBox.Show(string.Format("{0} sayısı ile {1} sayısının çarpma işlemi sonucu {2} çıkmıştır", s1, s2, s3));
                            break;
                        case Hesaplar.Bol:
                            s3 = s1 / s2;
                            MessageBox.Show(string.Format("{0} sayısı ile {1} sayısının bölme işlemi sonucu {2} çıkmıştır", s1, s2, s3));
                            break;
                    }
                    break;
            }
        }

    }
}
