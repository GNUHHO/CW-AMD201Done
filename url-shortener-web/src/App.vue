<template>
    <div class="container">
        <h1>🚀 URL Shortener</h1>
        <p class="subtitle">Rút gọn link của bạn trong 1 nốt nhạc</p>

        <div class="input-group">
            <input v-model="originalUrl"
                   type="url"
                   placeholder="Dán link dài vào đây (VD: https://greenwich...)"
                   @keyup.enter="shortenUrl" />
            <button @click="shortenUrl" :disabled="isLoading">
                {{ isLoading ? 'Đang xử lý...' : 'Rút gọn' }}
            </button>
        </div>

        <div v-if="shortUrl" class="result-box">
            <p>🎉 Thành công! Link của bạn đây:</p>
            <a :href="shortUrl" target="_blank" class="short-link">{{ shortUrl }}</a>
        </div>

        <p v-if="errorMessage" class="error-text">{{ errorMessage }}</p>

        <div v-if="history.length > 0" class="history-section">
            <h3>🕒 Lịch sử của bạn</h3>
            <div class="history-list">
                <div v-for="(item, index) in history" :key="index" class="history-item">
                    <a :href="item.shortUrl" target="_blank" class="history-short">{{ item.shortUrl }}</a>
                    <span class="history-arrow">⬅️</span>
                    <span class="history-original" :title="item.originalUrl">{{ item.originalUrl }}</span>
                </div>
            </div>
            <button class="clear-btn" @click="clearHistory">Xóa lịch sử</button>
        </div>
    </div>
</template>

<script setup>
    import { ref, onMounted } from 'vue';

    const originalUrl = ref('');
    const shortUrl = ref('');
    const errorMessage = ref('');
    const isLoading = ref(false);

    // Biến lưu trữ danh sách lịch sử
    const history = ref([]);

    // Khi trang web vừa mở lên, lấy dữ liệu từ LocalStorage ra hiển thị
    onMounted(() => {
        const savedHistory = localStorage.getItem('myUrlHistory');
        if (savedHistory) {
            history.value = JSON.parse(savedHistory);
        }
    });

    const shortenUrl = async () => {
        if (!originalUrl.value) return;

        isLoading.value = true;
        errorMessage.value = '';
        shortUrl.value = '';

        try {
            const response = await fetch('http://localhost:5078/api/shorten', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ url: originalUrl.value })
            });

            if (!response.ok) throw new Error('Không thể rút gọn link. Vui lòng kiểm tra lại!');

            const data = await response.json();
            shortUrl.value = data.shortUrl;

            // --- THÊM VÀO LỊCH SỬ ---
            // Đẩy link mới lên đầu danh sách
            history.value.unshift({
                originalUrl: originalUrl.value,
                shortUrl: data.shortUrl
            });

            // Lưu mảng mới vào LocalStorage của trình duyệt
            localStorage.setItem('myUrlHistory', JSON.stringify(history.value));

        } catch (error) {
            errorMessage.value = error.message;
        } finally {
            isLoading.value = false;
        }
    };

    // Hàm xóa lịch sử
    const clearHistory = () => {
        history.value = [];
        localStorage.removeItem('myUrlHistory');
    };
</script>

<style>
    body {
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        background-color: #f4f7f6;
        display: flex;
        justify-content: center;
        padding-top: 50px;
        padding-bottom: 50px;
        margin: 0;
    }

    .container {
        background: white;
        padding: 40px;
        border-radius: 12px;
        box-shadow: 0 10px 30px rgba(0,0,0,0.1);
        text-align: center;
        max-width: 650px;
        width: 100%;
    }

    h1 {
        color: #2c3e50;
        margin-bottom: 5px;
    }

    .subtitle {
        color: #7f8c8d;
        margin-bottom: 30px;
    }

    .input-group {
        display: flex;
        gap: 10px;
    }

    input {
        flex: 1;
        padding: 12px 15px;
        border: 2px solid #ddd;
        border-radius: 8px;
        font-size: 16px;
        outline: none;
    }

        input:focus {
            border-color: #42b883;
        }

    button {
        padding: 12px 25px;
        background-color: #42b883;
        color: white;
        border: none;
        border-radius: 8px;
        font-size: 16px;
        font-weight: bold;
        cursor: pointer;
        transition: background 0.3s;
    }

        button:hover:not(:disabled) {
            background-color: #33a06f;
        }

        button:disabled {
            background-color: #a8d5c2;
            cursor: not-allowed;
        }

    .result-box {
        margin-top: 30px;
        padding: 20px;
        background-color: #e8f5e9;
        border: 1px dashed #4caf50;
        border-radius: 8px;
    }

    .short-link {
        font-size: 20px;
        color: #2e7d32;
        font-weight: bold;
        text-decoration: none;
    }

    .error-text {
        color: #e74c3c;
        margin-top: 20px;
        font-weight: bold;
    }

    /* CSS MỚI CHO PHẦN LỊCH SỬ */
    .history-section {
        margin-top: 40px;
        text-align: left;
        border-top: 2px solid #eee;
        padding-top: 20px;
    }

        .history-section h3 {
            color: #34495e;
            font-size: 18px;
            margin-bottom: 15px;
        }

    .history-list {
        display: flex;
        flex-direction: column;
        gap: 10px;
        max-height: 300px;
        overflow-y: auto;
    }

    .history-item {
        display: flex;
        align-items: center;
        background: #f8f9fa;
        padding: 10px 15px;
        border-radius: 8px;
        font-size: 14px;
    }

    .history-short {
        color: #2980b9;
        font-weight: bold;
        text-decoration: none;
        white-space: nowrap;
    }

        .history-short:hover {
            text-decoration: underline;
        }

    .history-arrow {
        margin: 0 15px;
        font-size: 12px;
    }

    .history-original {
        color: #7f8c8d;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
        max-width: 300px; /* Cắt bớt link gốc nếu quá dài */
    }

    .clear-btn {
        margin-top: 15px;
        background-color: #e74c3c;
        padding: 8px 15px;
        font-size: 14px;
        width: 100%;
    }

        .clear-btn:hover {
            background-color: #c0392b;
        }
</style>