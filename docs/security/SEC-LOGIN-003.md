# SEC-LOGIN-003 — Giới hạn thử đăng nhập liên tiếp

**Nhóm:** rate limiting / kiểm soát tự động hóa. **Ưu tiên:** Critical. **Trạng thái:** Not Run; thiếu staging và ngưỡng đã xác nhận.

## Điều kiện trước khi thực thi

- Môi trường staging được phép kiểm thử, có tài khoản thử riêng hoặc chính sách rõ ràng cho tài khoản giả.
- Ngưỡng số lần gửi, cửa sổ thời gian và tín hiệu giới hạn (mã HTTP, nội dung lỗi hoặc CAPTCHA) được nhóm hệ thống xác nhận.
- Giới hạn số lần gửi và điều kiện dừng đã được phê duyệt. Không chạy trên trang production.

## Bước kiểm thử

1. Mở Login trên staging trong trình duyệt mới.
2. Gửi thông tin thử sai theo nhịp tuần tự và không vượt số lần đã được phê duyệt.
3. Dừng ngay khi xuất hiện CAPTCHA, khóa hoặc rate limiting.
4. Đối chiếu số lần gửi và phản hồi với ngưỡng đã xác nhận; ghi bằng chứng.

**Kết quả mong đợi:** tín hiệu giới hạn xuất hiện theo chính sách đã thống nhất; không giả định ngưỡng hoặc thông báo cụ thể trước khi có yêu cầu. Ca này chưa có phương thức NUnit vì điều kiện và oracle chưa được cung cấp. Kiểm thử giới hạn đăng nhập không đo được khả năng chịu DDoS của toàn hệ thống.

Tham chiếu: [OWASP WSTG — Weak Lock Out Mechanism](https://wstg.owasp.org/latest/4-Web_Application_Security_Testing/04-Authentication/03-Weak_Lock_Out_Mechanism/).
