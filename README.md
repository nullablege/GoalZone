# GoalZone

Bu proje, **M&Y Eğitim Akademi 10. Dönem Full Stack .NET Bootcamp** kapsamında geliştirdiğim 5. case çalışmasıdır.

🔗 Repo: https://github.com/nullablege/GoalZone/

---

## Proje Hakkında

GoalZone, futbol karşılaşmalarını takip etmeye yönelik geliştirilmiş bir web uygulamasıdır. Kullanıcılar maçları, fikstürü ve puan durumunu görüntüleyebilir; admin tarafında ise maç ve ilgili detaylar sisteme eklenebilir.

Projede kullanılan futbol verileri **SportMonks API** üzerinden gerçek olarak çekilmiştir.

> Not: API anahtarları ve dış servis verileri güvenlik nedeniyle projeye dahil edilmemiştir ve GitHub’a pushlanmamıştır.

---

## Özellikler

- Haftaya göre maçların listelenmesi  
- Canlı, tamamlanan ve yaklaşan maçların ayrıştırılması  
- Fikstür sayfası  
- Puan durumu hesaplama  
- Maç detaylarının görüntülenmesi  

**Admin Paneli:**
- Maç ekleme  
- Maç olayı ekleme  
- Oyuncu değişikliği ekleme  
- Maç istatistikleri ekleme  

---

## Kullanılan Teknolojiler

- ASP.NET Core 8  
- ASP.NET Core MVC  
- ASP.NET Core Web API  
- Entity Framework Core  
- SQL Server  
- Bootstrap  
- HTML / CSS / JavaScript  

---

## Proje Yapısı

### API Katmanı
Veri işlemlerinin gerçekleştirildiği kısımdır.  
Maçlar, takımlar, istatistikler ve diğer tüm veriler bu katman üzerinden yönetilir ve UI katmanına sunulur.

### UI Katmanı (WebUI)
Kullanıcının etkileşime geçtiği arayüzdür.  
API üzerinden alınan veriler burada işlenerek kullanıcıya sunulur.

---

## Ekran Görüntüleri

### Ana Sayfa
Haftalık maç özetleri ve karşılaşma durumları listelenmektedir.

<img width="1469" height="1263" alt="image" src="https://github.com/user-attachments/assets/29a49ec4-d9bb-413f-b208-0ab51d7317bd" />


---

### Fikstür
Maçlar gün ve hafta bazlı olarak görüntülenmektedir.

<img width="1627" height="1262" alt="image" src="https://github.com/user-attachments/assets/c131b994-916e-45af-9060-fe6f4fe3f3f5" />


---

### Puan Durumu
Takımların puan, averaj ve performans bilgileri listelenir.
<img width="1311" height="1265" alt="image" src="https://github.com/user-attachments/assets/78b8a333-504e-4037-8ca5-a7e4223ac555" />



---

### Maç Detayı
Seçilen karşılaşmaya ait detaylı bilgiler gösterilmektedir.

<img width="1345" height="1266" alt="image" src="https://github.com/user-attachments/assets/8691edc7-916a-46c0-85c1-d6fbde8353ce" />
<img width="1326" height="1259" alt="image" src="https://github.com/user-attachments/assets/5918140e-e9ce-4c20-a33f-1025ced8d553" />


---

### Admin Paneli
Sisteme maç ve maçla ilgili detayların eklenebildiği yönetim ekranıdır.

Maç Ekle :
<img width="1324" height="1262" alt="image" src="https://github.com/user-attachments/assets/ceb44a66-a868-4ad0-bfcc-caaff5115152" />

Olay Ekle : 
<img width="1183" height="1264" alt="image" src="https://github.com/user-attachments/assets/6a49602a-bdf4-48c1-af2c-762f4c83c7ee" />

İstatistik Ekle : 
<img width="1128" height="1260" alt="image" src="https://github.com/user-attachments/assets/579423de-5c2a-4271-99c8-3cdd71e9eb4b" />
