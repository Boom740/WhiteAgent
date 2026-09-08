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
- [X] [Sprite player Idle]  [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ] [status:: Done] [estimate:: 2h] [actual:: 1h] [status:: Done] [note:: -]
- [X] [Sprite player Walk]  [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ] [status:: Done] [estimate:: 2h] [actual:: 1h] [status:: Done] [note:: -]

### Story 2 — [As a player, I will attack to kill the enemy]

- [X] [ระบบรับดาเมจผ่าน Hitbox]  [owner:: ณัฏฐกิตติ์ นามอภิรมย์]  [estimate:: 3]  [status:: Done]
- [ ] [ระบบ Combo]  [owner:: ณัฏฐกิตติ์ นามอภิรมย์]  [estimate:: 2]  [status:: todo]
- [X] [Sprite enemy01 take damage]  [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 2h] [actual:: 2h] [status:: Done] [note:: -]
- [X] [Sprite enemy01 die]  [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 2h]  [status:: todo]

### Story 3 — [As a player, I will throw head to stunt the enemy]

- [X] [ระบบปาหัว]  [owner:: ณัฏฐกิตติ์ นามอภิรมย์]  [estimate:: 3]  [status:: Done]
- [X] [ระบบเก็บหัว]  [owner:: ณัฏฐกิตติ์ นามอภิรมย์]  [estimate:: 3]  [status:: Done]
- [ ] [Sprite enemy01 dizzy]  [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 2h] [actual:: -] [status:: todo] [note:: -]

### Story 4 — [As a player, I will take damage]

- [X] [ระบบ collision]  [owner:: วงศ์วรรธน์ พงค์จินะ]  [estimate:: 3]  [status:: Done]
- [X] [Enemy01 Ai]  [owner:: ณัฏฐกิตติ์ นามอภิรมย์]  [estimate:: 4]  [status:: Done]
- [ ] [Sprite enemy01 attack]  [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 2h] [actual:: 1h] [status:: todo] [note:: -]
- [X] [Sprite player take damage]  [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 2h] [actual:: 1h] [status:: Done] [note:: -]

### Story 5 — [As a player, I will get upgrade]

- [ ] [ระบบ Upgrade] [domain::programmer] [owner:: วงศ์วรรธน์ พงค์จินะ]  [estimate:: 3]  [status:: todo]
- [ ] [UI design] [domain::Artist] [owner:: กนต์ระพี เดชะ]  [estimate:: 2]  [status:: todo]
- [ ] [UI Upgrade] [domain::Artist] [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 12h] [status:: todo]

### Story 6 — [As a player, I will fight more enemy]

- [ ] [Enemy02 Ai] [domain::programmer] [owner:: วงศ์วรรธน์ พงค์จินะ]  [estimate:: 4]  [status:: todo]
- [ ] [Enemy03 Ai] [domain::programmer] [owner:: วงศ์วรรธน์ พงค์จินะ]  [estimate:: 4]  [status:: todo]
- [ ] [Enemy04 Ai] [domain::programmer] [owner:: ณัฏฐกิตติ์ นามอภิรมย์]  [estimate:: 3]  [status:: todo]
- [ ] [Sprite enemy02] [domain::Artist] [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 12h] [status:: todo]
- [ ] [Sprite enemy03] [domain::Artist] [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 12h] [status:: todo]
- [ ] [Sprite enemy04] [domain::Artist] [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 12h] [status:: todo]

### Story 7 — [As a player, I will rather item]

- [ ] [Ui collectible item] [domain::Artist] [owner:: กนต์ระพี เดชะ]  [estimate:: 2]  [status:: todo]
- [ ] [collectible system] [domain::programmer] [owner:: วงศ์วรรธน์ พงค์จินะ]  [estimate:: 2]  [status:: todo]

---

## Daily Notes

### [2026/09/08]

**สัปดาห์ที่ผ่านมาได้ทำอะไรไปบ้าง:** ออกแบบตัวละคร, ออกแบบระบบเกม, วาด sprite player เสร็จแล้ว

**สัปดาห์นี้จะทำอะไรต่อไป:** ทำ sprite enemy ตัวที่เหลือเพิ่มเติม, ทำ sprite Ui และ Upgrade Ui

**พบปัญหาหรืออุปสรรคอะไรในการทำงาน:** ต้องสลับไปมาระหว่าง Designer กับ Artist ทำให้สับสนบางครั้ง

---

## Links

- [[docs/gdd/00-concept|GDD Concept]]
- [[docs/agile/01-product-backlog|Product Backlog]]
- [[docs/agile/02-sprint-backlog|Sprint Backlog]]
