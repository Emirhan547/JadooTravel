# ✈️ JadooTravel

**JadooTravel**, destinasyon keşfi, rezervasyon yönetimi ve operasyonel kontrol süreçlerini tek bir platformda birleştiren çok katmanlı bir seyahat web uygulamasıdır.

Proje; son kullanıcı deneyimi ile operasyonel yönetim ihtiyaçlarını dengeli şekilde ele alan, genişlemeye açık bir mimari üzerine inşa edilmiştir.

---

## 🌍 Temel Özellikler

### 👤 Kullanıcı Tarafı

* Destinasyon keşfi ve detaylı içerik görüntüleme
* Rezervasyon oluşturma ve durum takibi
* Favori listesi yönetimi
* Yorum bırakma ve topluluk geri bildirimi
* Rol bazlı kimlik doğrulama
* OpenAI destekli öneri kurgusu

---

### 🛠 Admin Paneli

* Destination, Category, Feature, TripPlan, Partner yönetimi
* Rezervasyon operasyon takibi
* Yorum moderasyonu
* Dashboard metrikleri
* Elasticsearch tabanlı audit log ekranları

---

## 🧠 Domain Modeli

Uygulama, seyahat deneyimi etrafında şekillenmiş temel varlıkları içerir:

* **Destination**
* **Booking**
* **DestinationReview**
* **Category**
* **Feature**
* **TripPlan**
* **Partner**
* **UserFavorite**
* **Testimonial**

Bu model; keşif, karar ve rezervasyon sürecini tutarlı bir domain dili altında birleştirir.

---

## ⚙️ Teknik Stack

* **ASP.NET Core MVC (.NET 9)**
* **MongoDB**
* **MongoDB Repository Pattern**
* **ASP.NET Core Identity (MongoDB Store)**
* **AutoMapper**
* **AWS S3** (Medya saklama)
* **OpenAI API**
* **Elasticsearch**
* **Kibana**
* **Docker (Elasticsearch & Kibana container orchestration)**

---

## 🐳 Docker Kullanımı

Elasticsearch ve Kibana servisleri Docker üzerinden ayağa kaldırılmaktadır.

Bu yapı sayesinde:

* Audit log altyapısı ortamdan bağımsız çalışır
* Lokal geliştirme ortamı hızlı kurulabilir
* Monitoring servisleri uygulama kodundan izole kalır

---

## 🔐 Güvenlik & İzlenebilirlik

* ASP.NET Core Identity ile rol bazlı yetkilendirme
* MongoDB tabanlı kimlik saklama
* Kritik operasyonların Elasticsearch üzerinden audit kaydı
* Kibana ile log analizi ve görselleştirme
* Index odaklı MongoDB yaklaşımı

---

<img width="1905" height="863" alt="Ekran görüntüsü 2026-02-27 140042" src="https://github.com/user-attachments/assets/31e0f1c0-1ede-4a14-a0b8-ffb0f7c77fc1" />


<img width="847" height="777" alt="Ekran görüntüsü 2026-02-27 140111" src="https://github.com/user-attachments/assets/314f7561-6e84-4046-8f24-9af8db60f6e3" />


<img width="1904" height="852" alt="Ekran görüntüsü 2026-02-27 031010" src="https://github.com/user-attachments/assets/14cd0408-eebe-4431-b1c0-3503ed4824a1" />


<img width="1889" height="863" alt="Ekran görüntüsü 2026-02-27 141653" src="https://github.com/user-attachments/assets/db69fd43-0070-47ac-af5a-a945dd1b89a0" />


<img width="829" height="851" alt="Ekran görüntüsü 2026-02-27 142108" src="https://github.com/user-attachments/assets/38160983-e456-4c6c-b18b-42b4df7d25dd" />

