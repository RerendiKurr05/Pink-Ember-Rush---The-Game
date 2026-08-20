# Pink Ember Rush The-Game
**2D Pixel-Art Platformer — Arcade Survival / High-Rush**
---

## Deskripsi

Ini adalah dev repo untuk **Pink Ember Rush**, sebuah game platformer survival ritme-cepat: pemain bermain sebagai pemuda yang berlari, melompat, dan menghindar dari musuh sambil mengumpulkan **Cat Pink** untuk mengisi **Ember** dan memberantas musuh yang menginvasi daerah kalian.

Isi Ember sampai penuh dan ambil **Power-Up** untuk meluncurkan serangan balik. Semakin lama bertahan, semakin banyak musuh muncul — skor akhir ditentukan oleh waktu bertahan dan efisiensi, dan membiarkan kalian untuk mengelola risiko vs imbalan, dibungkus dalam nuansa kota Jember yang playful dan romantis.

Latar cerita: Kota Jember dikenal sebagai **"Kota Cinta"**. Warna-warna cinta di kota mulai pudar karena gangguan makhluk-makhluk kelabu bernama **Grim Sprites**. Karakter utama, seorang pelukis jalanan kecil bernama **Emba**, harus berlari mengumpulkan Cat Pink yang tersisa untuk mengisi Ember Cinta dan mengembalikan warna kota.

| Aspek | Ketentuan |
|---|---|
| Genre | 2D Platformer — Arcade Survival / High-Rush |
| Sudut Pandang | Side-scrolling 2D, kamera mengikuti pemain (Cinemachine) |
| Engine | Unity (2D URP, Pixel Perfect Camera package) |
| Platform Rilis | PC (Windows) dahulu; potensi porting Android setelahnya |
| Target Pemain | Casual–core, penggemar arcade survival (mis. Vampire Survivors, Downwell), usia 13+ |
| Durasi Sesi | Singkat & berulang: 2–6 menit per sesi ("one more run") |
| Mode | Single-player, fokus leaderboard skor tertinggi (lokal via PlayerPrefs) |

---

## Status Development

Core loop dan fondasi teknis sudah berfungsi end-to-end dari Main Menu sampai Game Over. Yang **sudah diimplementasikan**:

- Pergerakan dasar (base Tarodev Ultimate 2D Controller) + Dash + Wall Mechanics
- **Grappling Hook** — berayun dengan momentum via `DistanceJoint2D`
- Core loop Ember & Cat Pink (`PlayerPaintManager`, `PaintCollectible`, `Powerup`)
- Sistem kombat area (`PlayerAttack` + `Physics2D.OverlapCircleAll`)
- AI musuh berbasis inheritance — `EnemyBase` → `EnemyGround` / `EnemyFlying`
- Efek partikel hati ala *Tame Wolf* Minecraft (Particle System, bukan Rigidbody drop)
- **Game Juice**: Hit Stop, Camera Shake (Cinemachine Noise), Slow-Motion + audio teredam (Audio Low Pass Filter)
- Sistem gelombang musuh berbasis waktu (`EnemySpawner`)
- Parallax background landmark Kota Jember
- Timer bertahan hidup + High Score permanen (`PlayerPrefs`) + panel Game Over & Restart
- Main Menu (Play, Settings, Tutorial, Developer Info, Quit + konfirmasi Yes/No)
- Pause Menu in-game (Resume, Settings, Quit to Menu + konfirmasi)

Yang **belum** (Menurut Technical Addendum atau GDD):

- AI musuh "menyerang" yang benar-benar men-set `isAttacking = true` untuk memicu Counter-Attack
- Level design (susunan tanah, rintangan, titik grapple)
- Playtesting & balancing angka baseline
- Animasi karakter final (saat ini masih placeholder warna sprite)
- Porting Android + kontrol on-screen

### Pilar Desain

- **Risk vs Reward yang jelas** — mengumpulkan cat berarti mendekat ke musuh, tapi itulah satu-satunya jalan menuju kekuatan menyerang.
- **Tekanan yang meningkat progresif** — timer pertandingan adalah musuh sesungguhnya; jumlah musuh bertambah secara bertahap dan terasa adil.
- **Kepuasan visual & taktil** — perubahan warna musuh, ledakan hati ala Minecraft, Hit Stop, Camera Shake, dan Slow-Motion memberi umpan balik instan atas setiap aksi pemain.
- **Identitas lokal yang hangat** — kota Jember sebagai "Kota Cinta" menjadi kanvas visual lewat parallax background landmark yang dikenali.

---

## Prasyarat & Instalasi

### Prasyarat

- [Unity Hub](https://unity.com/download) dengan **Unity LTS terbaru** (target render pipeline: **2D URP**)
- Git & [Git LFS](https://git-lfs.com/) (disarankan untuk aset pixel-art, audio, dan file besar lainnya)
- Package Unity yang dipakai proyek ini:
  - `2D Sprite`, `2D Animation`, `2D Pixel Perfect`
  - `Cinemachine` (kamera follow + Camera Shake via Noise 6D Wobble)
  - `Input System`
  - `TextMeshPro` (Import TMP Essentials saat prompt pertama muncul)
- [Ultimate-2D-Controller](https://github.com/Matthew-J-Spencer/Ultimate-2D-Controller) (Matthew J. Spencer) sebagai fondasi movement — lihat langkah instalasi di bawah

### Instalasi

```bash
# 1. Clone repo
git clone <URL_REPO_INI>
cd pink-ember-rush

# 2. (Jika pakai Git LFS) tarik aset besar
git lfs pull
```

1. Buka **Unity Hub** → `Add project from disk` → pilih folder hasil clone.
2. Pastikan versi Unity yang terpasang sesuai `ProjectSettings/ProjectVersion.txt`.
3. Buka proyek — Unity akan mengimpor package & aset otomatis pada pembukaan pertama.
4. Buka scene `Assets/Scenes/MainMenu.unity` lalu tekan **Play** untuk mulai dari Main Menu, atau `MainGame.unity` untuk langsung ke gameplay.

### Membuat struktur folder dari nol

Jika memulai proyek Unity baru dari kosong, struktur folder standar bisa dibuat otomatis lewat salah satu cara berikut (kode lengkap ada di Lampiran A Technical Addendum):

- **Opsi 1 (direkomendasikan):** Letakkan `FolderGenerator.cs` di `Assets/Editor/`, lalu di Unity pilih menu **Tools → Generate Pink Ember Folders**.
- **Opsi 2:** Jalankan file batch Windows (`.bat`) yang membuat folder proyek beserta strukturnya langsung dari File Explorer sebelum Unity dibuka.

---

## Contoh Penggunaan

Menjalankan game dari Unity Editor:

```
Unity Editor → buka scene MainMenu → tekan tombol Play → klik "Play" di menu
```

Membuat build PC (Windows):

```
File → Build Settings → tambahkan MainMenu (indeks 0) & MainGame (indeks 1) ke Scenes In Build
→ pilih platform Windows → Build
```

### Kontrol (PC, versi terkini)

| Aksi | Input | Catatan |
|---|---|---|
| Gerak kiri/kanan | `A` / `D` atau Panah Kiri/Kanan | Base movement dari Tarodev Controller |
| Lompat | `Space` | Coyote time + jump buffering |
| Dash | `Shift` | Evasive move singkat, ber-cooldown |
| **Grappling Hook** | `E` / Klik Kanan | Mengait titik `GrapplePoint` terdekat, berayun membawa momentum |
| Serang (khusus Ember Mode) | Klik Kiri Mouse / `J` | Hanya aktif saat Powerup/Ember Mode berjalan |
| Pause | `Esc` | Membekukan game & membuka Pause Menu |

Kontrol lengkap juga ditampilkan di panel **Tutorial** pada Main Menu in-game.

---

## Struktur Folder Proyek

```
Assets/
├── Scenes/            MainMenu.unity, MainGame.unity
├── Sprites/            Jember_Backgrounds/, Characters/, UI_Items/
├── Materials/          Particle_Material.mat
├── Prefabs/            Enemies/, Collectibles/, Effects/
├── Scripts/
│   ├── Core/           GameManager.cs, GameJuiceManager.cs
│   ├── Player/         PlayerPaintManager.cs, PlayerAttack.cs, PlayerHealth.cs
│   ├── Enemy/          EnemyBase.cs, EnemyGround.cs, EnemyFlying.cs, EnemySpawner.cs
│   ├── Environment/     ParallaxBackground.cs, PaintCollectible.cs, Powerup.cs
│   ├── Effects/         AutoDestroyParticle.cs
│   ├── Menu/            MainMenuManager.cs, PauseManager.cs
│   └── Editor/          FolderGenerator.cs (tidak ikut build)
└── TarodevController/   Base movement controller pihak ketiga
```

Detail tanggung jawab tiap script, tag/layer yang dipakai, dan parameter default harus mengikuti ketentuan yang ada.

---

## Sistem Inti (ringkasan arsitektur)

Proyek menggunakan pola **event-driven** dan **inheritance** agar antar-sistem longgar keterkaitannya:

| Sistem | Skrip Utama | Pola |
|---|---|---|
| Core loop Ember/Cat | `PlayerPaintManager`, `PaintCollectible`, `Powerup` | Direct reference + method call |
| Kombat | `PlayerAttack` | `Physics2D.OverlapCircleAll` per layer `Enemy` |
| AI Musuh | `EnemyBase` → `EnemyGround` / `EnemyFlying` | Inheritance, `Move()` di-override |
| Game Juice | `GameJuiceManager` (singleton) | Dipanggil dari `PlayerAttack` & `PlayerController` |
| Gelombang Musuh | `EnemySpawner` | Time-based, bukan score-based |
| Skor & Game Over | `GameManager`, `PlayerHealth` | `PlayerPrefs` untuk High Score permanen |
| Menu | `MainMenuManager`, `PauseManager` | Panel `SetActive()` show/hide |

Untuk angka-angka desain (ambang cat, durasi power-up, tabel wave, dsb.), gunakan **ScriptableObject konfigurasi terpusat** — jangan hardcode di skrip — agar mudah di-tuning saat playtest tanpa mengubah kode.

---

## Roadmap Produksi (Milestone)

| Milestone | Fokus Utama | Status |
|---|---|---|
| M0 | Prototype inti: gerak, platform dasar, musuh dummy | Selesai |
| M1 | Loop Ember lengkap: Ember UI, Paint Stock, spawn/despawn power-up | Selesai |
| M2 | Kombat & musuh: pinkify progress, heart burst, tipe musuh | Selesai |
| M3 | Skala kesulitan & skor: EnemySpawner, ScoreManager, Game Over flow | Selesai |
| M3.5 | *(baru)* Game Juice, Grappling Hook, Main Menu & Pause Menu | Selesai |
| M4 | Art pass Jember: parallax landmark, palet warna final | Sebagian (parallax sudah, palet final belum) |
| M5 | Audio & polish: musik adaptif, SFX lengkap, UI polish | Belum |
| M6 | Playtest & Balancing | Belum |
| M7 | Rilis Awal (v1.0) | Belum |

---

## Kontribusi

Kontribusi sangat terbuka, terutama pada tahap art pass, level design, dan balancing. Alur yang disarankan:

1. **Fork** repo ini, lalu buat branch baru dari `main`:
   ```bash
   git checkout -b fitur/nama-fitur-singkat
   ```
2. Ikuti struktur skrip & pola event-driven/inheritance yang sudah ada — hindari reference langsung antar sistem yang tidak perlu.
3. Untuk angka-angka desain, gunakan **ScriptableObject konfigurasi terpusat**, jangan hardcode di skrip.
4. Baca **Technical Addendum bab 10 (Development Log)** sebelum mengubah `EnemyBase.cs` — beberapa variabel (mis. `isAttacking`) sudah dideklarasikan sebagai fondasi untuk fitur mendatang, jangan dihapus.
5. Commit dengan pesan yang jelas, lalu buka **Pull Request** ke `main` dengan deskripsi:
   - Apa yang diubah/ditambahkan
   - Milestone terkait (lihat tabel roadmap)
   - Cara mengujinya di Editor
6. Laporkan bug atau ajukan ide lewat **Issues**, sertakan langkah reproduksi bila relevan.

---

## Lisensi

Belum ditentukan secara resmi. Sampai lisensi ditetapkan, seluruh kode, aset, dan dokumen desain dalam repo ini dianggap **hak cipta milik tim pengembang** — mohon hubungi pemilik repo sebelum menyalin, memodifikasi, atau mendistribusikan ulang di luar keperluan kontribusi pada proyek ini.

---

## Developer Info

```
PINK EMBER RUSH
Developer 1: Muhammad Rendi Kurniawan
Developer 2: Martarina Rachmi Nafilah
Developer 3: Nabil Zivkolin Danendra
Mahasiswa Manajemen Informatika (MIF) Angkatan 2024

contact: mrendikurniawaninbox@gmail.com
```
