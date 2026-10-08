# SEC-LOGIN-004 — Duy trì dịch vụ trong tải đã phê duyệt

**Nhóm:** khả năng chống quá tải / DDoS resilience. **Ưu tiên:** Normal. **Trạng thái:** Not Run; chưa có môi trường, load profile hoặc SLA.

## Điều kiện trước khi thực thi

- Môi trường cô lập được phép kiểm thử; không dùng trang production hay tài khoản thật.
- Kế hoạch tải mô tả lưu lượng nền, đỉnh tải, thời lượng, số máy phát tải và giới hạn tài nguyên.
- SLA về độ trễ, tỉ lệ lỗi và độ sẵn sàng, cùng ngưỡng dừng an toàn, được phê duyệt trước.
- Có người phụ trách giám sát hạ tầng trong suốt lần chạy.

## Bước kiểm thử

1. Ghi nhận baseline của dịch vụ trên môi trường cô lập.
2. Chạy load profile đã phê duyệt bằng công cụ kiểm thử hiệu năng phù hợp.
3. Theo dõi độ trễ, tỉ lệ lỗi, tài nguyên và khả năng truy cập của người dùng thử.
4. Dừng ngay tại ngưỡng an toàn; so sánh số đo với SLA đã thống nhất.

**Kết quả mong đợi:** đạt SLA đã thống nhất và không ảnh hưởng người dùng khác. Hiện chưa có cơ sở để gán số lượng request, thời gian, ngưỡng lỗi hoặc trạng thái Passed/Failed. Selenium UI không phải công cụ tạo tải DDoS; ca này được giữ trong Excel như một test case cần môi trường và kế hoạch riêng.

Tham chiếu về kiểm soát tự động hóa và giới hạn: [OWASP Bot Management and Anti-Automation Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Bot_Management_and_Anti-Automation_Cheat_Sheet.html).
