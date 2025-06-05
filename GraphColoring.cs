using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GraphColoringApp
{
    public class GraphColoring
    {
        private List<State> states;
        private bool[,] adjacencyMatrix;
        private int numStates;
        private int[] colors;
        private int minColorsNeeded = int.MaxValue;
        private int[] bestColorsFound;

        public GraphColoring(List<State> states, bool[,] adjacencyMatrix)
        {
            this.states = states;
            this.adjacencyMatrix = adjacencyMatrix;
            this.numStates = states.Count;
            this.colors = new int[numStates];
            this.bestColorsFound = new int[numStates];

            for (int i = 0; i < numStates; i++)
            {
                colors[i] = -1;
                bestColorsFound[i] = -1;
            }
        }

        private bool IsSafe(int stateId, int color)
        {
            for (int i = 0; i < numStates; i++)
            {
                if (adjacencyMatrix[stateId, i] && colors[i] == color)
                {
                    return false;
                }
            }
            return true;
        }
        private bool IsValidColoring()
        {
            for (int i = 0; i < numStates; i++)
            {
                for (int j = i + 1; j < numStates; j++)
                {
                    // Якщо є ребро між i та j
                    if (adjacencyMatrix[i, j])
                    {
                        // Перевіряємо, чи не мають вони однакового кольору
                        if (colors[i] == colors[j] && colors[i] != -1)
                        {
                            Debug.WriteLine($"ПОМИЛКА: Вершини {i} ({states[i].Name}) та {j} ({states[j].Name}) мають однаковий колір {colors[i]}");
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        // ВИПРАВЛЕНА евристика MRV
        private int GetNextStateMRV(int maxColorsCount)
        {
            int minRemainingColors = int.MaxValue;
            int nextState = -1;

            for (int i = 0; i < numStates; i++)
            {
                if (colors[i] == -1) // Якщо вершина не розфарбована
                {
                    int availableColorsCount = 0;

                    // Рахуємо скільки кольорів з 0 до maxColorsCount-1 можна використати
                    for (int color = 0; color < maxColorsCount; color++)
                    {
                        if (IsSafe(i, color))
                        {
                            availableColorsCount++;
                        }
                    }

                    // Якщо немає доступних кольорів, повертаємо цю вершину як найбільш обмежену
                    if (availableColorsCount == 0)
                    {
                        return i;
                    }

                    // Вибираємо вершину з найменшою кількістю доступних кольорів
                    if (availableColorsCount < minRemainingColors)
                    {
                        minRemainingColors = availableColorsCount;
                        nextState = i;
                    }
                }
            }
            return nextState;
        }

        // СПРОЩЕНИЙ алгоритм backtracking
        private bool SolveGraphColoringRecursive(int statesColoredCount, int maxColorsCount)
        {
            if (statesColoredCount == numStates)
            {
                return true;
            }

            // Використовуємо виправлену MRV евристику
            int stateId = GetNextStateMRV(maxColorsCount);

            if (stateId == -1)
            {
                return true; // Всі вершини розфарбовані
            }

            // Спробуємо кольори від 0 до maxColorsCount-1
            for (int color = 0; color < maxColorsCount; color++)
            {
                if (IsSafe(stateId, color))
                {
                    colors[stateId] = color;

                    if (SolveGraphColoringRecursive(statesColoredCount + 1, maxColorsCount))
                    {
                        return true;
                    }

                    colors[stateId] = -1; // Backtrack
                }
            }

            return false;
        }

        // АЛЬТЕРНАТИВНИЙ швидкий алгоритм (жадібний)
        private (int chromaticNumber, Dictionary<string, int> coloring) SolveGreedy()
        {
            Debug.WriteLine("Використовуємо жадібний алгоритм для швидкого розв'язання...");

            // Скидаємо кольори
            for (int i = 0; i < numStates; i++)
            {
                colors[i] = -1;
            }

            // Сортуємо вершини за кількістю сусідів (по спаданню)
            var sortedStates = Enumerable.Range(0, numStates)
                .OrderByDescending(i => GetDegree(i))
                .ThenBy(i => i) // Додаткове сортування для стабільності
                .ToArray();

            int maxColorUsed = -1;

            foreach (int stateId in sortedStates)
            {
                // Знаходимо найменший доступний колір
                bool colorFound = false;
                for (int color = 0; color <= maxColorUsed + 1; color++)
                {
                    if (IsSafe(stateId, color))
                    {
                        colors[stateId] = color;
                        if (color > maxColorUsed)
                        {
                            maxColorUsed = color;
                        }
                        colorFound = true;
                        Debug.WriteLine($"Вершина {stateId} ({states[stateId].Name}) отримала колір {color}");
                        break;
                    }
                }

                if (!colorFound)
                {
                    Debug.WriteLine($"ПОМИЛКА: Не вдалося знайти колір для вершини {stateId}");
                }
            }

            // Перевіряємо правильність розфарбовування
            if (!IsValidColoring())
            {
                Debug.WriteLine("УВАГА: Розфарбовування містить помилки!");
                return (int.MaxValue, new Dictionary<string, int>());
            }

            // Створюємо результат
            Dictionary<string, int> resultColoring = new Dictionary<string, int>();
            for (int i = 0; i < numStates; i++)
            {
                resultColoring[states[i].Name] = colors[i];
                states[i].Color = colors[i];
            }

            Debug.WriteLine($"Жадібний алгоритм завершено. Використано {maxColorUsed + 1} кольорів.");
            return (maxColorUsed + 1, resultColoring);
        }

        private int GetDegree(int stateId)
        {
            int degree = 0;
            for (int i = 0; i < numStates; i++)
            {
                if (adjacencyMatrix[stateId, i])
                {
                    degree++;
                }
            }
            return degree;
        }

        // ГОЛОВНИЙ метод з таймаутом
        public (int chromaticNumber, Dictionary<string, int> coloring) FindChromaticNumberAndColoring()
        {
            Debug.WriteLine("Запуск алгоритму розфарбовування графа...");

            // Спочатку пробуємо оптимальний backtracking з обмеженням часу
            var stopwatch = Stopwatch.StartNew();
            const int timeoutMs = 5000; // 5 секунд таймаут

            for (int k = 1; k <= Math.Min(numStates, 8); k++) // Обмежуємо максимальну кількість кольорів
            {
                if (stopwatch.ElapsedMilliseconds > timeoutMs)
                {
                    Debug.WriteLine($"Таймаут досягнуто після {stopwatch.ElapsedMilliseconds}мс. Переходимо на жадібний алгоритм.");
                    break;
                }

                Debug.WriteLine($"Спроба з {k} кольорами...");

                // Скидаємо кольори
                for (int i = 0; i < numStates; i++)
                {
                    colors[i] = -1;
                }

                if (SolveGraphColoringRecursive(0, k))
                {
                    Debug.WriteLine($"Знайдено оптимальне розв'язання з {k} кольорами!");
                    minColorsNeeded = k;
                    bestColorsFound = (int[])colors.Clone();

                    Dictionary<string, int> resultColoring = new Dictionary<string, int>();
                    for (int i = 0; i < numStates; i++)
                    {
                        resultColoring[states[i].Name] = bestColorsFound[i];
                        states[i].Color = bestColorsFound[i];
                    }

                    return (minColorsNeeded, resultColoring);
                }
            }

            // Якщо оптимальний алгоритм не знайшов розв'язок або вийшов час, використовуємо жадібний
            Debug.WriteLine("Використовуємо жадібний алгоритм як резервний варіант.");
            return SolveGreedy();
        }

        private void PrintAdjacencyInfo()
        {
            Debug.WriteLine("=== МАТРИЦЯ СУМІЖНОСТІ ===");
            for (int i = 0; i < numStates; i++)
            {
                List<int> neighbors = new List<int>();
                for (int j = 0; j < numStates; j++)
                {
                    if (adjacencyMatrix[i, j])
                    {
                        neighbors.Add(j);
                    }
                }
                Debug.WriteLine($"Вершина {i} ({states[i].Name}): сусіди = [{string.Join(", ", neighbors)}]");
            }
        }

        // API інтеграція (без змін)
        private static readonly HttpClient client = new HttpClient();
        private const string GoogleMapsApiKey = "AIzaSyD9mVRn0yLm0U2oPM4T4HsIiAS69no2ZVU";

        public static async Task GetStateCoordinatesAsync(List<State> states)
        {
            Debug.WriteLine("\nОтримання координат штатів за допомогою Google Maps Geocoding API...");
            foreach (var state in states)
            {
                string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(state.Name + ", Mexico")}&key={GoogleMapsApiKey}";
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        JsonElement root = doc.RootElement;
                        string status = root.GetProperty("status").GetString();

                        if (status == "OK")
                        {
                            if (root.TryGetProperty("results", out JsonElement resultsElement) &&
                                resultsElement.GetArrayLength() > 0 &&
                                resultsElement[0].TryGetProperty("geometry", out JsonElement geometryElement) &&
                                geometryElement.TryGetProperty("location", out JsonElement locationElement))
                            {
                                state.Latitude = locationElement.GetProperty("lat").GetDouble();
                                state.Longitude = locationElement.GetProperty("lng").GetDouble();
                                Debug.WriteLine($"Координати {state.Name}: Широта={state.Latitude}, Довгота={state.Longitude}");
                            }
                            else
                            {
                                Debug.WriteLine($"Помилка: Не вдалося знайти координати для {state.Name}");
                            }
                        }
                        else
                        {
                            string errorMessage = root.TryGetProperty("error_message", out JsonElement errorMsgElement) ? errorMsgElement.GetString() : "Немає детального повідомлення про помилку.";
                            Debug.WriteLine($"Помилка геокодування для {state.Name}: Статус: {status}. Деталі: {errorMessage}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Помилка для {state.Name}: {e.Message}");
                }
                await Task.Delay(200);
            }
        }
    }
}