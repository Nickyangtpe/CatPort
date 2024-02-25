#include <iostream>
#include <string>
#include <curl/curl.h>

// 回调函数，用于接收HTTP响应数据
size_t writeCallback(void* contents, size_t size, size_t nmemb, std::string* output) {
    size_t total_size = size * nmemb;
    output->append((char*)contents, total_size);
    return total_size;
}

int main() {
    std::cout << "\033[1;35m"; // 设置文本颜色为紫色
    std::cout << R"(
 $$$$$$\              $$\     $$$$$$$\                        $$\     
$$  __$$\             $$ |    $$  __$$\                       $$ |    
$$ /  \__| $$$$$$\  $$$$$$\   $$ |  $$ | $$$$$$\   $$$$$$\  $$$$$$\   
$$ |       \____$$\ \_$$  _|  $$$$$$$  |$$  __$$\ $$  __$$\ \_$$  _|  
$$ |       $$$$$$$ |  $$ |    $$  ____/ $$ /  $$ |$$ |  \__|  $$ |    
$$ |  $$\ $$  __$$ |  $$ |$$\ $$ |      $$ |  $$ |$$ |        $$ |$$\ 
\$$$$$$  |\$$$$$$$ |  \$$$$  |$$ |      \$$$$$$  |$$ |        \$$$$  |
 \______/  \_______|   \____/ \__|       \______/ \__|         \____/  
)" << std::endl;
    std::cout << "\033[0m"; // 恢复默认文本颜色


    std::string response;
    // 用户输入IP地址和端口号
    std::string ip_address;
    int port_number;
    std::cout << "Enter IP address: ";
    std::cin >> ip_address;
    std::cout << "Enter port number: ";
    std::cin >> port_number;

    // 用户输入重复次数
    int repeat_count;
    std::cout << "Enter number of repetitions: ";
    std::cin >> repeat_count;

    // 初始化Curl
    CURL* curl = curl_easy_init();
    if (curl) {
        // 构建URL
        std::string url = "http://" + ip_address + ":" + std::to_string(port_number);
        curl_easy_setopt(curl, CURLOPT_URL, url.c_str());

        // 设置接收响应数据的回调函数
        curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, writeCallback);
        curl_easy_setopt(curl, CURLOPT_WRITEDATA, &response);

        // 执行请求多次
        CURLcode res;
        for (int i = 0; i < repeat_count; ++i) {
            res = curl_easy_perform(curl);
            if (res != CURLE_OK) {
                std::cerr << "Failed to perform HTTP request: " << curl_easy_strerror(res) << std::endl;
            }
            else {
                // 打印响应数据
                std::cout << "Response " << i + 1 << ": " << response << std::endl;
            }
            // 清空响应数据以备下一次使用
            response.clear();
        }

        // 清理Curl资源
        curl_easy_cleanup(curl);
    }

    return 0;
}
