# VPK Okuyucu

CS2 oyununun dosyalarının içini okuyor.

## Neden yazdım

Oynadığım oyunlarda yer sıkıntısı çekmeye başladım. Bu yüzden içlerinde ne olduğunu merak ettim, aslında hep bir fikrim vardı. Hiçbir zaman üstüne çok düşmemiştim ama bu sefer farklıydı; merakım beni çok cezbedince nasıl yapılacağını araştırdım ve ilgili şeyleri öğrendim.

## Nasıl çalışıyor

VPK'nın wikisine girdim, oradan neyin ne olduğunu anlamaya çalıştım. İlk başta hiçbir şey anlamadım ama okuyarak ve çevirerek, aslında olması gereken her şeyi oyunlara göre verdiklerini anladım. Sonrasında kod gibi görünen ama kod olmayan tarifler ile bana kendi dilimde yazmam için şema verdiklerini gördüm. Hata alıp almadığımı kontrol etmem için default veya olması gereken değerleri vermişler, onlara göre kontrolümü de yaptım. Kaydırmadım, hata almadım.

- İsimleri 0 baytına kadar okuyup yazıya çevirdim. Uzantı → klasör → dosya diye iç içe üç döngüyle dolaştım.
- Her kaydın sonunda `0xFFFF` gelmesi gerekiyor. Gelmezse program "KAYMA VAR" yazıyor.

## Ne buldum

137.734 kayıt okudum. Oyun dosyalarında en çok yer tutan üç şeyi fark ettim: oyunun texture'ları yani dokuları (42,97 GB), seslerin kapladığı yer (4,34 GB) ve oyundaki nesnelerin modelleri (0,56 GB).

## Video

https://youtu.be/0_2k9qnJ1SM

## Çalıştırma

```bash
dotnet run
```

`Program.cs` 3. satırdaki `yol` değişkenini kendi `pak01_dir.vpk` yolunla değiştir.
