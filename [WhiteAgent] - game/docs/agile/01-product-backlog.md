# Product Backlog

**Version:** 1.0 | **Last Updated:** 2026-09-01

> รวม User Story ทั้งหมดของโปรเจกต์ — ยังไม่ได้แปลว่าต้องทำใน Sprint นี้ทั้งหมด
> โปรเจกต์นี้แบ่งงานตลอดเทอมเป็น **4 Sprint** (Sprint 1-4) — Sprint ไหนหยิบ Story ไปทำ ให้ใส่เลข Sprint นั้น (1-4) ลงคอลัมน์ `Sprint`

## Must Have (MVP)

| # | User Story                                          | Acceptance Criteria                                                                                                                                                                | Estimate (SP) | Sprint |
| - | --------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------- | ------ |
| 1 | As a player, I will attack to kill the enemy       | กดปุ่มโจมตีแล้วศัตรูได้รับดาเมจเลือดลดจนตายได้                                                                                       | 3             | 1      |
| 2 | As a player, I will throw head to stunt the enemy | ทำการปาหัวออกไปแล้วศัตรูเข้าสู่สถานะมึนงง และหัวเด้งออกจากตัวศัตรูทันที                                      | 4             | 1      |
| 3 | As a player, I will take damage                     | เมื่อศัตรูโจมตีแล้วเลือดของผู้เล่นลดลงตามดาเมจที่ได้รับ                                                                     | 1             | 1      |
| 4 | As a player, I will get upgrade                     | เมื่อผู้เล่นเคลียร์เลเวลแล้ว จะมีอัปเกรดขึ้นมาให้เลือก                                                                        | 1             | 1      |
| 5 | As a player, I want to move                         | เมื่อผู้เล่นกด WASD ตัวละครต้องเคลื่อนที่ตามทิศทางที่กดจริงๆ                                                                 | 2             | 1      |
| 6 | As a player, I want to save game                    | เมื่อผู้เล่นไปยังเลเวลถัดไป จะทำการบันทึกตำแหน่งไว้ ถ้าผู้เล่นตายด่านไหนจะเกิดใหม่ด่านนั้น | 1             | 2      |
| 7 |                                                     |                                                                                                                                                                                    |               |        |
| 8 |                                                     |                                                                                                                                                                                    |               |        |

## Should Have

| # | User Story                              | Acceptance Criteria                                                    | Estimate (SP) | Sprint |
| - | --------------------------------------- | ---------------------------------------------------------------------- | ------------- | ------ |
| 1 | As a designer, I want to make Main menu | มีปุ่มเริ่มเกม มีปุ่มออกเกม มี How to play | 2             | 3      |
| 2 | -                                       | -                                                                      | -             | -      |

## Nice to Have

| # | User Story                                                                                                      | Acceptance Criteria                                                                                                                                      | Estimate (SP) | Sprint |
| - | --------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------- | ------ |
| 1 | As a designer, I want enemy spawn rate stored in a data file, so that I can tune difficulty without recompiling | ปรับค่า spawn rate ในไฟล์ data แล้วรันเกมใหม่ ค่าที่เปลี่ยนมีผลทันทีโดยไม่ต้อง build ใหม่ | 5             | 3      |
| 2 | As a designer, I want enemy spawn in random position                                                            | ทุกครั้งที่เล่น ศัตรูจะเกิดไม่ซ้ำที่เดิม                                                                          | 5             | 3      |

## MoSCoW Legend

- **Must Have** — จำเป็นต่อ core gameplay loop เกมเล่นไม่ได้ถ้าขาด (MVP)
- **Should Have** — เพิ่มคุณภาพเกม แต่เกมเล่นได้โดยไม่มีก็ได้
- **Nice to Have** — ทำถ้ามีเวลาเหลือ

## Links

- [[docs/gdd/00-concept|GDD Concept]]
- [[docs/agile/02-sprint-backlog|Sprint Backlog]].
