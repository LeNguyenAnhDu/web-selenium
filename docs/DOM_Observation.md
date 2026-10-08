# Khảo sát trang đăng nhập UTC

Khảo sát bằng GET HTML và Chrome 154 headless ngày 08/10/2026 (UTC+07:00), trước khi viết assertion. URL: <https://vanphongdientu.utc.edu.vn/Login>.

## DOM đã quan sát

| Thành phần | Locator / thuộc tính quan sát |
|---|---|
| Form | `form[action='/Login'][method='post']` |
| Username | `input[name='username']`, `type='text'` |
| Password | `input[name='userpwd']`, `type='password'` |
| Submit | `input.submit_login[type='submit']` trong form |
| Lỗi sau POST | `form[action='/Login'] div.error` |

Hai ô không có `required` hoặc `maxlength` trong DOM GET. Sau POST, `validationMessage` của cả hai ô rỗng. Không suy ra giới hạn độ dài nào. `div.error` chỉ xuất hiện sau phản hồi server. Form và URL `/Login` vẫn hiển thị sau các lần thất bại, nhưng assertion dùng `div.error` cùng sự hiện diện của form; URL không dùng làm bằng chứng duy nhất.

## Hành vi đã quan sát

| Dữ liệu | Phản hồi trong `div.error` | Phân loại |
|---|---|---|
| Cả hai trống | `Bạn chưa nhập tên đăng nhập` | Validation server |
| Username giả, password trống | `Bạn chưa nhập mật khẩu` | Validation server |
| Username trống, password giả | `Bạn chưa nhập tên đăng nhập` | Validation server |
| Hai trường có dữ liệu giả | `Tài khoản hoặc mật khẩu không đúng.` | Xác thực server |
| Hai trường chỉ có ba dấu cách | `Tài khoản hoặc mật khẩu không đúng.` | Xác thực server theo hành vi hiện tại |

Username giả: `utc_qa_nonexistent_20261008`; password giả chỉ dùng trong mã và workbook. Đây là dữ liệu dành riêng cho kiểm thử, không có tài khoản hợp lệ. Các thông báo là **hành vi quan sát**, không được coi là đặc tả sản phẩm bất biến. Không tạo ca trim khoảng trắng đầu/cuối vì chưa có yêu cầu rõ ràng. Không tạo ca vượt độ dài vì chưa xác định giới hạn.

Khảo sát gửi form có giới hạn và dừng nếu có CAPTCHA, khóa hoặc rate limiting. Lần chạy NUnit cũng có giới hạn năm lần gửi form, chạy tuần tự.
