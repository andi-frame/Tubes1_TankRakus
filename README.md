# Robocode Tank Royale - TankRakus

## Deskripsi 
Repository ini berisi bot yang dibuat oleh kelompok TankRakus memenuhi tugas besar mata kuliah IF2211 - Strategi Algoritma

## Algoritma greedy yang digunakan
1. Greedy Damage Peluru
2. Greedy Kill Musuh
3. Greedy Damage (Ramming dan Peluru)
4. Greedy Survival Time

## Requirement
- .Net
- IDE, teks edior, atau terminal
- Starter Pack yang tersedia pada link berikut: 
https://github.com/Ariel-HS/tubes1-if2211-starter-pack 
Downloadlah asset dibawah dari release terbaru.
- Download jar “robocode-tankroyale-gui-0.30.0.jar”, yang merupakan game 
engine hasil modifikasi asisten.

## Cara menjalankan bot
1. Jalankan file .jar aplikasi GUI dan mengetikan perintah berikut pada terminal
   ```
   java -jar robocode-tankroyale-gui-0.30.0.jar
   ```
2. Setup konfigurasi booter
   a. Klik tombol “Config”
   b. Klik tombol “Bot Root Directories”
   c. Masukkan directory yang berisi folder-folder bot kalian 
3. Jalankan sebuah battle
   a. Klik tombol “Battle”
   b. Klik tombol “Start Battle”
   c. Akan muncul panel konfigurasi permainan
      1. Bot-bot di dalam directory yang telah disetup pada proses konfigurasi akan otomatis muncul pada kotak kiri-atas
   d. Boot bot yang ingin Anda mainkan
      1. Select bot yang ingin dimainkan pada kotak kiri-atas
      2. Klik tombol “Boot →”
      3. Bot yang telah diselect akan muncul pada kotak kanan-atas 
      4. Bot yang berhasil di-boot akan muncul pada kotak kiri-bawah
   e. Tambahkan bot ke dalam permainan
      1. Select bot yang ingin ditambahkan ke dalam permaianan pada kotak kiri-bawah
      2. Klik tombol “Add →”
      3. Apabila Anda ingin menambahkan semua bot, klik tombol “Add All →”
      4. Bot yang telah ditambahkan akan otomatis muncul pada kotak kanan-bawah
      5. Mulai permainan dengan menekan tombol “Start Battle” 

DISCLAIMER
Ada beberapa kasus bot tidak bisa dijalankan, coba cek kode berikut pada file .csproj
```
<Project Sdk="Microsoft.NET.Sdk"> 
    <PropertyGroup> 
        <RootNamespace>BotTemplate</RootNamespace> 
        <OutputType>Exe</OutputType> 
        <TargetFramework>net6.0</TargetFramework> 
        <LangVersion>10.0</LangVersion> 
    </PropertyGroup> 
    <ItemGroup> 
        <PackageReference Include="Robocode.TankRoyale.BotApi" 
Version="0.30.0"/> 
    </ItemGroup> 
</Project>
```

1. RootNamespace berisi nama Class dari bot Anda (e.g. BotTemplate). 
2. TargetFramework berisi versi .Net version yang diperlukan bot untuk 
dijalankan, yaitu versi .Net Anda. Anda dapat melihat versi .Net (dotnet) 
Anda dengan menjalani command “dotnet --version” 
3. LangVersion berisi versi bahasa C# untuk menjalankan bot Anda 
4. PackageReference berisi versi Robocode.TankRoyale.BotApi yang 
digunakan. Untuk Tugas Besar ini, versi yang digunakan adalah “0.30.0” 

## Author
Andi Farhan Hidayat - 13523128
Andri Nurdianto - 13523145
Muhammad Farrel Wibowo - 13523153
