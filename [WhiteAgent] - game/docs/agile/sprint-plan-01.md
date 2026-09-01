<!-- Template เต็มไฟล์สำหรับสร้าง docs/agile/sprint-plan-[NN].md ของ Sprint ไหนก็ได้ -->

<!-- ดึง Story ของ Sprint นี้มาจาก docs/agile/02-sprint-backlog.md -->

<!-- Sprint 1: เปลี่ยนชื่อ sprint-01.md จาก Lab 07 เป็น sprint-plan-01.md แล้วแทนที่เนื้อหาด้วย template นี้ -->

<!-- Sprint 2-4 ในแลปถัดไป: คัดลอกไฟล์นี้ทั้งไฟล์ไปสร้าง sprint-plan-02.md, sprint-plan-03.md, sprint-plan-04.md ตามลำดับ -->

# Sprint [N] Plan

**Sprint Goal:** [เป้าหมายหลักของ Sprint นี้ในหนึ่งประโยค]
**ระยะเวลา:** [2026/09/01] — [2026/10/17]
**Team:** [ธีรภัทร ศิริณัฐกุลสมบัติ, วงศ์วรรธน์ พงค์จินะ, กนต์ระพี เดชะ, ณัฏฐกิตติ์ นามอภิรมย์]

---

## Sprint Backlog

| # | User Story                                          | รับผิดชอบ                                                                           | MoSCoW    | Estimate (SP) | Status  |
| - | --------------------------------------------------- | -------------------------------------------------------------------------------------------- | --------- | ------------- | ------- |
| 1 | As a player, I want to move                         | [วงศ์วรรธน์ พงค์จินะ, ธีรภัทร ศิริณัฐกุลสมบัติ]     | Must Have | [2]           | ✅ Done |
| 2 | As a player, I will attack to kill the enemy       | [ณัฏฐกิตติ์ นามอภิรมย์, ธีรภัทร ศิริณัฐกุลสมบัติ] | Must Have | [3]           | 🔲 Todo |
| 3 | As a player, I will throw head to stunt the enemy | [ณัฏฐกิตติ์ นามอภิรมย์, ธีรภัทร ศิริณัฐกุลสมบัติ] | Must Have | [4]           | 🔲 Todo |
| 4 | As a player, I will take damage                     | [วงศ์วรรธน์ พงค์จินะ]                                                      | Must Have | [1]           | 🔲 Todo |
| 5 | As a player, I will get upgrade                     | [วงศ์วรรธน์ พงค์จินะ, กนต์ระพี เดชะ]                           | Must Have | [1]           | 🔲 Todo |

## Status Legend

- 🔲 Todo
- 🔄 In Progress
- ✅ Done
- ❌ Blocked

---

## Tasks

### Story 1 — [As a player, I want to move]

- [X] [ระบบเคลื่อนที่ตามทิศทางความเร็ว]  [owner:: วงศ์วรรธน์ พงค์จินะ]  [estimate:: 3]  [status:: Done]
- [ ] [Sprite player Idle]  [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 2]  [status:: doing]
- [ ] [Sprite player Walk]  [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 2]  [status:: doing]

### Story 2 — [As a player, I will attack to kill the enemy]

- [ ] [ระบบรับดาเมจผ่าน Hitbox]  [owner:: ณัฏฐกิตติ์ นามอภิรมย์]  [estimate:: 3]  [status:: doing]
- [ ] [ระบบ Combo]  [owner:: ณัฏฐกิตติ์ นามอภิรมย์]  [estimate:: 2]  [status:: doing]
- [ ] [Sprite enemy01 take damage]  [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 2]  [status:: doing]
- [ ] [Sprite enemy01 die]  [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 2]  [status:: doing]

### Story 3 — [As a player, I will throw head to stunt the enemy]

- [ ] [ระบบปาหัว]  [owner:: ณัฏฐกิตติ์ นามอภิรมย์]  [estimate:: 3]  [status:: doing]
- [ ] [ระบบเก็บหัว]  [owner:: ณัฏฐกิตติ์ นามอภิรมย์]  [estimate:: 3]  [status:: doing]
- [ ] [Sprite enemy01 dizzy]  [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 2]  [status:: doing]

### Story 4 — [As a player, I will take damage]

- [X] [ระบบ collision]  [owner:: วงศ์วรรธน์ พงค์จินะ]  [estimate:: 3]  [status:: Done]
- [ ] [Enemy01 Ai]  [owner:: ณัฏฐกิตติ์ นามอภิรมย์]  [estimate:: 4]  [status:: doing]
- [ ] [Sprite enemy01 attack]  [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 2]  [status:: doing]
- [ ] [Sprite player take damage]  [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 2]  [status:: doing]

### Story 5 — [As a player, I will get upgrade]

- [ ] [ระบบแสดง UI]  [owner:: วงศ์วรรธน์ พงค์จินะ]  [estimate:: 2]  [status:: doing]
- [ ] [ระบบอัปเดตค่า status]  [owner:: วงศ์วรรธน์ พงค์จินะ]  [estimate:: 2]  [status:: doing]
- [ ] [UI design]  [owner:: กนต์ระพี เดชะ]  [estimate:: 2]  [status:: doing]
- [ ] [UI Upgrade sprite]  [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 2]  [status:: doing]

---

## Daily Notes

### [2026/09/01]

**เมื่อวาน:**
**วันนี้:** ...
**Blocked:** ...

---

## Links

- [[docs/gdd/00-concept|GDD Concept]]
- [[docs/agile/01-product-backlog|Product Backlog]]
- [[docs/agile/02-sprint-backlog|Sprint Backlog]]
