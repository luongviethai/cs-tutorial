FROM postgres:17-alpine

# Giá trị mặc định, có thể ghi đè bằng biến môi trường khi chạy container.
# TZ được initdb dùng để set timezone của server khi tạo database lần đầu.
ENV POSTGRES_DB=cs_tutorial \
    POSTGRES_USER=postgres \
    TZ=Asia/Ho_Chi_Minh

HEALTHCHECK --interval=10s --timeout=5s --start-period=10s --retries=5 \
    CMD pg_isready -U "$POSTGRES_USER" -d "$POSTGRES_DB" || exit 1

EXPOSE 5432
