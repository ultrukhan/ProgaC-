# ProgaC-
---
## Жоский репозиторій для навчання

##### Для початку роботи потрібно стягнути репозиторій. Перед кожним стягуванням останніх змін переключатись на **main**, роботу бажано виконувати на своїй гілці, потім просто змерджимо

### 1. Клонування репозиторію (лише один раз)
`git clone https://github.com/ultrukhan/ProgaC-.git`

### 2. Створення власної гілки
Створюємо нову гілку для кожного нового завдання:
`git checkout -b назва_нової_гілки`

### 3. Збереження змін
Коли виконали частину роботи:
```
git add .
git commit -m "Опис того, що саме змінено"
git push origin назва_твоєї_гілки
```

### 4. Оновлення репозиторію
Перед тим як почати працювати, стягуємо актуальний код, який додали інші:
Переходимо на main та оновлюємо його
```
git checkout main
git pull origin main
```
Повертаємось у свою гілку та вбираємо в неї зміни з main
```
git checkout назва_твоєї_гілки
git merge main
```

### 5. Створення нового проекту
- <img width="51" height="56" alt="image" src="https://github.com/user-attachments/assets/8c7024d6-860e-4829-99e0-c21cc9f2556c" /> Знаходимо таку іконку і натискаємо
- обираємо Folder View
- <img width="997" height="45" alt="image" src="https://github.com/user-attachments/assets/2d0b44a8-281f-435e-8d3e-4349d5fb625a" /> Обираємо File
- <img width="722" height="816" alt="image" src="https://github.com/user-attachments/assets/9e16b17a-fe3e-4488-b3be-3de3d39c9eed" /> Обираємо Add -> New Project
- Налаштовуємо проект, як і зазвичай
- <img width="613" height="659" alt="image" src="https://github.com/user-attachments/assets/724dbe8c-c2e3-49c2-849a-233630481494" /> Двічі натискаємо на ProgaC-.slnx
- <img width="599" height="1112" alt="image" src="https://github.com/user-attachments/assets/3c805f13-c1b6-4027-8fd7-30448f20028a" /> Правою кнопкою миші на новостворений проект і обираємо Set as Startup Project
- Після цих дій все запускатиметься





