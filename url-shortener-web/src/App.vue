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
    </div>
</template>

<script setup>
    import { ref } from 'vue';

    const originalUrl = ref('');
    const shortUrl = ref('');
    const errorMessage = ref('');
    const isLoading = ref(false);

    const shortenUrl = async () => {
        if (!originalUrl.value) return;

        isLoading.value = true;
        errorMessage.value = '';
        shortUrl.value = '';

        try {
            // Gọi thẳng vào API .NET của bạn ở port 5078
            const response = await fetch('http://localhost:5078/api/shorten', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ url: originalUrl.value })
            });

            if (!response.ok) throw new Error('Không thể rút gọn link. Vui lòng kiểm tra lại!');

            const data = await response.json();
            shortUrl.value = data.shortUrl;
        } catch (error) {
            errorMessage.value = error.message;
        } finally {
            isLoading.value = false;
        }
    };
</script>

<style>
    body {
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        background-color: #f4f7f6;
        display: flex;
        justify-content: center;
        padding-top: 100px;
        margin: 0;
    }

    .container {
        background: white;
        padding: 40px;
        border-radius: 12px;
        box-shadow: 0 10px 30px rgba(0,0,0,0.1);
        text-align: center;
        max-width: 600px;
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

        .short-link:hover {
            text-decoration: underline;
        }

    .error-text {
        color: #e74c3c;
        margin-top: 20px;
        font-weight: bold;
    }
</style>