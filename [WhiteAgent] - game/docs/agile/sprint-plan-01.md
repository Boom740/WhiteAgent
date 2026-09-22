<!-- Template เต็มไฟล์สำหรับสร้าง docs/agile/sprint-plan-[NN].md ของ Sprint ไหนก็ได้ -->

<!-- ดึง Story ของ Sprint นี้มาจาก docs/agile/02-sprint-backlog.md -->

<!-- Sprint 1: เปลี่ยนชื่อ sprint-01.md จาก Lab 07 เป็น sprint-plan-01.md แล้วแทนที่เนื้อหาด้วย template นี้ -->

<!-- Sprint 2-4 ในแลปถัดไป: คัดลอกไฟล์นี้ทั้งไฟล์ไปสร้าง sprint-plan-02.md, sprint-plan-03.md, sprint-plan-04.md ตามลำดับ -->

# Sprint [1] Plan

**Sprint Goal:** [เป้าหมายหลักของ Sprint นี้ในหนึ่งประโยค]
**ระยะเวลา:** [2026/08/31] — [2026/09/12]
**Team:** [ธีรภัทร ศิริณัฐกุลสมบัติ, วงศ์วรรธน์ พงค์จินะ, กนต์ระพี เดชะ, ณัฏฐกิตติ์ นามอภิรมย์]

---

## Sprint Backlog

| # | User Story                                          | รับผิดชอบ                                                                           | MoSCoW    | Estimate (SP) | Status  |
| - | --------------------------------------------------- | -------------------------------------------------------------------------------------------- | --------- | ------------- | ------- |
| 1 | As a player, I want to move                         | [วงศ์วรรธน์ พงค์จินะ, ธีรภัทร ศิริณัฐกุลสมบัติ]     | Must Have | [2]           | ✅ Done |
| 2 | As a player, I will attack to kill the enemy       | [ณัฏฐกิตติ์ นามอภิรมย์, ธีรภัทร ศิริณัฐกุลสมบัติ] | Must Have | [3]           | ✅ Done |
| 3 | As a player, I will throw head to stunt the enemy | [ณัฏฐกิตติ์ นามอภิรมย์, ธีรภัทร ศิริณัฐกุลสมบัติ] | Must Have | [4]           | ✅ Done |
| 4 | As a player, I will take damage                     | [วงศ์วรรธน์ พงค์จินะ]                                                      | Must Have | [1]           | ✅ Done |

## Status Legend

- 🔲 Todo
- 🔄 In Progress
- ✅ Done
- ❌ Blocked

---

## Tasks

### Story 1 — [As a player, I want to move]

- [X] [ระบบเคลื่อนที่ตามทิศทางความเร็ว]  [owner:: วงศ์วรรธน์ พงค์จินะ]  [estimate:: 3]  [status:: Done]
- [X] [Sprite player Idle]  [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 2]  [status:: Done]
- [X] [Sprite player Walk]  [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 2]  [status:: Done]

### Story 2 — [As a player, I will attack to kill the enemy]

- [X] [ระบบรับดาเมจผ่าน Hitbox]  [owner:: ณัฏฐกิตติ์ นามอภิรมย์]  [estimate:: 3]  [status:: Done]
- [X] [ระบบ Combo]  [owner:: ณัฏฐกิตติ์ นามอภิรมย์]  [estimate:: 2]  [status:: Done]

### Story 3 — [As a player, I will throw head to stunt the enemy]

- [X] [ระบบปาหัว]  [owner:: ณัฏฐกิตติ์ นามอภิรมย์]  [estimate:: 3]  [status:: Done]
- [X] [ระบบเก็บหัว]  [owner:: ณัฏฐกิตติ์ นามอภิรมย์]  [estimate:: 3]  [status:: Done]

### Story 4 — [As a player, I will take damage]

- [X] [ระบบ collision]  [owner:: วงศ์วรรธน์ พงค์จินะ]  [estimate:: 3]  [status:: Done]
- [X] [Enemy01 Ai]  [owner:: ณัฏฐกิตติ์ นามอภิรมย์]  [estimate:: 4]  [status:: Done]
- [X] [Sprite player take damage]  [owner:: ธีรภัทร ศิริณัฐกุลสมบัติ]  [estimate:: 2]  [status:: Done]


## Daily Notes

### [2026/09/08]

**สัปดาห์ที่ผ่านมาได้ทำอะไรไปบ้าง:** เน้นทำระบบของ player และทำให้ enemy ออกมาให้รับดาเมจได้แล้ว

**สัปดาห์นี้จะทำอะไรต่อไป:** ทำ enemy ตัวที่เหลือเพิ่มเติม, ระบบ Ui, และระบบ Upgrade

**พบปัญหาหรืออุปสรรคอะไรในการทำงาน:** Programmer ทำงานไม่ทันตาม Deadline เพราะว่าเจอปัญหาที่ยังไม่รู้วิธีแก้ ต้องไปศึกษาเพิ่มเติม

---

## Links

- [[docs/gdd/00-concept|GDD Concept]]
- [[docs/agile/01-product-backlog|Product Backlog]]
- [[docs/agile/02-sprint-backlog|Sprint Backlog]]
