# hata.md — vpk_okuyucu

## 27 Ekim 2026 — Yazıyı sayı olarak okumak
- **Ne oldu:** Yol ve dosya adı için `ReadUInt32()` çağrıldı (Program.cs:30, 32). Çıktı `1869504880`, `1634558322` oldu, `panorama/videos/test` yerine.
- **Neden:** `ReadUInt32` 4 byte'ı sayı olarak okur. Yol ve ad ise `00` ile biten yazı (wiki'de `string`). Yazı alanı için kendi metodumuz `BinaryOkuyucu` kullanılmalı.
- **Nasıl fark edildi:** Beklenen yazı yerine sayı çıktı. `1869504880` = hex `6F6E6170` → ters byte'lar "pano".
- **Düzeltme:** Wiki'deki türe bak: `string` → `BinaryOkuyucu`, `int` → `ReadUInt32`, `short` → `ReadUInt16`.

## 27 Ekim 2026 — Aynı hata, 2 byte'la
- **Ne oldu:** `ReadUInt32` yerine `ReadUInt16` yazıldı (Program.cs:30, 32). Çıktı `24944` ("pa"), `28526` ("no").
- **Neden:** Sorun byte sayısında değil, türde. Yazı alanı sayı okuyan hiçbir metotla okunmaz. Muhtemel kök: `BinaryOkuyucu`'nun tekrar çağrılabileceği ve her çağrıda kaldığı yerden devam ettiği oturmamış.
- **Düzeltme:** `a.BinaryOkuyucu(OlusanNesne)` satırını 2 kez daha çağırmak.

## 27 Ekim 2026 — Eski deneme satırları kayması
- **Ne oldu:** 6 alanın okuma satırları doğru yazıldı ama `ArchiveIndex` 141 yerine 16898 çıktı.
- **Neden:** Önceki denemelerden kalan iki satır okuyucuyu ileri itti: (1) `var olusanNesne10 = OlusanNesne.ReadUInt16();` "panorama"nın "pa"sını yedi, (2) 3. `BinaryOkuyucu` çağrısı (`olusanAtaNesne3`) CRC'nin byte'larını (`0e 2a c2 2a 00`) yazı diye yedi. Tüm sayılar 5 byte kaydı.
- **Nasıl fark edildi:** Kontrol noktası 141 tutmadı.
- **Ders:** Okuyucu geri gitmez; her fazla okuma sonraki her şeyi kaydırır. Deneme satırlarını işi bitince sil. Kontrol noktaları (141, 65535) tam da bunu yakalamak için.

## 27 Ekim 2026 — Yanlış satırlar yoruma alındı
- **Ne oldu:** Fazla satırları silmek yerine dosya adı okuması (`olusanAtaNesne2`) da yoruma alındı, `ReadUInt16` (satır 30) kaldı. CRC = 1735290977 = "apng" (dosya adı "apngtest"in ilk 4 harfi).
- **Neden:** Hangi satırın hangi alanı okuduğu takip edilmedi; değişken adları (`olusanAtaNesne1/2/3`) anlamı söylemiyor.
- **Ders:** Değişkene okuduğu şeyin adını ver (`uzanti`, `klasorYolu`, `dosyaAdi`) — hangi satırın neyi okuduğu bir bakışta görünür.

## 27 Ekim 2026 — Onluk sayıyı ikişer basamak bölmek
- **Ne oldu:** `Terminator`'dan sonra `ReadUInt32` ile `1835139072` okundu; byte'larını bulmak için onluk basamaklar ikişer bölündü: `72 90 13 35 18`.
- **Neden:** Byte sınırları yalnız 16'lık (hex) yazımda görünür: her 2 hex hane = 1 byte. Onluk yazımda basamaklar byte'lara denk gelmez. Ayrıca "2'lik sistem" dendi; kastedilen 16'lık.
- **Düzeltme:** Önce sayıyı hex'e çevir (`printf '%X\n' 1835139072`), sonra ikişer böl, sonra ters çevir (little-endian).
- **Not:** Burada yine yazı olabilecek yere sayı okundu (3. kez, bkz. ilk kayıt). Yer yazı mı sayı mı bilinmiyorsa `ReadUInt32` + hex'e bakmak deneme olarak işe yarar; ama kalıcı kodda `BinaryOkuyucu`.

## 27 Ekim 2026 — Ters çevirirken byte'ın içini de çevirmek
- **Ne oldu:** `6D 62 00 00` ters çevrilirken haneler de ters çevrildi: `6D` → `D6` = 214, `62` → `26` = 38.
- **Neden:** Little-endian BYTE sırasını ters çevirir, byte'ın içindeki iki haneyi değil. `6D` bir bütün, ters çevrilince yine `6D`. Doğrusu `00 00 62 6D`.
- **Ek:** "`00` = nokta" sanıldı. `xxd`'nin sağ sütununda basılamayan her byte `.` görünür; `00` nokta değil, yazının BİTTİĞİ işaret (`BinaryOkuyucu`'daki `if (b == 0) break;`). Gerçek nokta `2E`.

## 27 Ekim 2026 — Dizin döngülerinde yazı yerine tek byte okumak
- **Ne oldu:** `BinaryOkuyucuDongulu` içinde 3 iç içe `while` kuruldu (yapı doğru) ama her seviyede `binary.ReadByte()` çağrıldı.
- **Neden:** İki ayrı döngü karıştı. `ReadString` (bizim `BinaryOkuyucu`) byte byte dönen döngü; dizin döngüleri (uzantı/klasör/dosya) ise her turda BÜTÜN bir yazı okur. `ReadByte` "apng" yerine yalnız "a"yı okur.
- **Düzeltme:** Her seviyede yazıyı `BinaryOkuyucu(binary)` ile oku, `== ""` ile kontrol et.

## 27 Ekim 2026 — Toplamı yanlış döngü seviyesinde basmak
- **Ne oldu:** `tutucu` rafta tanımlandı (doğru) ama `WriteLine($"su an tutucu kacta...")` orta (kutu) döngünün İÇİNE yazıldı.
- **Neden:** Bir satırın hangi döngüde olduğunu kapanış parantezi belirler. Kağıt döngüsünün `}`'sinin altı = hâlâ kutu döngüsü. Sonuç: her kutu sonunda ara toplam basılır.
- **Düzeltme:** Satırı bir `}` aşağı, kutu döngüsünün dışına taşı → raf başına tek satır.
- **Kural:** Tanımladığın yer = sıfırlandığı yer; bastığın yer = kaç kez basıldığı.

## 27 Ekim 2026 — Sayaç orta döngüye taşındı → okuyucu kaydı
- **Ne oldu:** `while (i < 4)` dış döngüden kutu döngüsüne taşındı. Çıktı: `gif: 0.00`, sonra `panorama/videos/test`, `giftest`, `öÔú` raf diye basıldı.
- **Neden:** 4 raftan sonra `i = 4` oldu, kutu döngüsüne hiç girilmedi → `gif` rafının kutu ve kağıtları OKUNMADI. Dış döngü bir sonraki yazıyı (klasör yolu) raf sandı, ardından dosya adını, sonra CRC byte'larını → çöp.
- **Ders:** Döngüyü atlamak "okumamak" demek; okunmayan her byte sonraki okumaları kaydırır (bkz. eski deneme satırları kaydı). Sınırlama gerekiyorsa sınırı DIŞ döngüye koy.

## 27 Ekim 2026 — Tam sayı bölmesi ve eksik birim
- **Ne oldu:** `{tutucu / 1024:f2}` → `apng: 652.00` (gerçek 652,56 KB).
- **Neden:** (1) `1024` tam sayı; `ulong / int` tam sayı bölmesi yapar, küsurat atılır, `:f2` ancak `.00` ekleyebilir. (2) Tek bölme byte → KB yapar, GB değil.
- **Düzeltme:** `tutucu / 1024.0 / 1024 / 1024` (`.0` = virgüllü bölme, üç bölme = GB).

## 26 Ekim 2026 — (Claude) Tarih hatası
- **Ne oldu:** Claude oturum boyunca "bugün 27 Ekim, sınır bugün akşam" dedi. Gerçek tarih 26 Ekim.
- **Neden:** Günlükteki "27 EKİM — YARININ LİSTESİ" başlığı bugünün listesi sanıldı; sistemdeki tarih kontrol edilmedi.
- **Nasıl fark edildi:** Kullanıcı söyledi.
- **Etki:** Sahte bir son tarih baskısı yarattı. Gerçek sınır: 27 Ekim akşamı.

## 28 Ekim 2026 — Koşullu toplamayı eklerken eski toplamayı bırakmak
- **Ne oldu:** `if (ArchiveIndex == 144) { tutucu += EntryLength; }` eklendi ama döngünün sonundaki eski `tutucu += EntryLength;` satırı silinmedi.
- **Neden:** Yeni satır eskisinin YERİNE geçmeliydi. İkisi birden durunca diğer parçalardaki her kayıt yine toplanıyor, 144'tekiler ise iki kez sayılıyor. Sonuç dünkü toplamlardan bile büyük çıkar, hata sessizce geçer.
- **Düzeltme:** Alttaki koşulsuz `tutucu += EntryLength;` satırını sil.
- **Kural:** Bir satırı koşula bağlarken koşulsuz kopyası kalmasın; "taşıdım mı, kopyaladım mı?" diye kontrol et.
