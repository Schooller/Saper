// SaperA.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <iostream>
#include <ctime>
#include <string>
#include <conio.h>
#include <Windows.h>

using namespace std;

bool keyPlay = false, keyGame = false, retGame = false, keywin =false;
string level = "Beginner";

class Game
{
private:
	int **ArrN, NumBomb, Line, Column, num1 = 0, num2 = 0;
	char **FuncArr;
	bool **LettArr, **CoorCur;
	enum eDiraction { LEFT, RIGHT, UP, DOWN, BOMB, SET, EXIT };
	eDiraction dir;
	HANDLE  hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
public:
	void createArrs(int ShadowNumBomb, int ShadowLine, int ShadowColumn)
	{
		NumBomb = ShadowNumBomb;
		Line = ShadowLine;
		Column = ShadowColumn;
		ArrN = new int*[ShadowLine];
		for (int i = 0; i < ShadowLine; i++) {
			ArrN[i] = new int[ShadowColumn];
		}
		for (int i = 0; i < Line; i++)
		{
			for (int j = 0; j < Column; j++)
			{
				ArrN[i][j] = 0;
			}
		}
		FuncArr = new char*[ShadowLine];
		for (int i = 0; i < ShadowLine; i++) {
			FuncArr[i] = new char[ShadowColumn];
		}
		for (int i = 0; i < Line; i++)
		{
			for (int j = 0; j < Column; j++)
			{
				FuncArr[i][j] = 'D';
			}
		}
		LettArr = new bool*[ShadowLine];
		for (int i = 0; i < ShadowLine; i++)
		{
			LettArr[i] = new bool[ShadowColumn];
		}
		for (int i = 0; i < Line; i++)
		{
			for (int j = 0; j < Column; j++)
			{
				LettArr[i][j] = false;
			}
		}
		CoorCur = new bool*[ShadowLine];
		for (int i = 0; i < ShadowLine; i++)
		{
			CoorCur[i] = new bool[ShadowColumn];
		}
		for (int i = 0; i < Line; i++)
		{
			for (int j = 0; j < Column; j++)
			{
				CoorCur[i][j] = false;
			}
		}
		CoorCur[0][0] = true;
	}
	/////////////////////////////////////////////////////////////////////////////
	void setBomb()
	{
		int numB1 = 0, numB2 = 0;
		for (int i = 0; i<NumBomb; i++)
		{
			numB1 = rand() % (Line - 1);
			numB2 = rand() % (Column - 1);
			if (ArrN[numB1][numB2] != 9)
			{
				ArrN[numB1][numB2] = 9;
			}
			else
			{
				i--;
			}
		}
	}
	void setNums()
	{
		int sum = 0;
		for (int i = 0; i < Line; i++)
		{
			for (int j = 0; j < Column; j++)
			{
				sum = 0;
				if (ArrN[i][j] != 9) {
					if (i + 1 < Line) {
						if (ArrN[i + 1][j] == 9)
						{
							sum++;
						}
					}
					if (j + 1 < Column) {
						if (ArrN[i][j + 1] == 9)
						{
							sum++;
						}
					}
					if (i + 1 < Line&&j + 1 < Column) {
						if (ArrN[i + 1][j + 1] == 9)
						{
							sum++;
						}
					}
					if (i - 1 >= 0) {
						if (ArrN[i - 1][j] == 9)
						{
							sum++;
						}
					}
					if (j - 1 >= 0) {
						if (ArrN[i][j - 1] == 9)
						{
							sum++;
						}
					}
					if (j - 1 >= 0 && i - 1 >= 0) {
						if (ArrN[i - 1][j - 1] == 9)
						{
							sum++;
						}
					}
					if (i + 1 < Line&&j - 1 >= 0) {
						if (ArrN[i + 1][j - 1] == 9)
						{
							sum++;
						}
					}
					if (i - 1 >= 0 && j + 1 < Column) {
						if (ArrN[i - 1][j + 1] == 9)
						{
							sum++;
						}
					}
					ArrN[i][j] = sum;
				}
			}
		}
	}
	void getFunction()
	{
		switch (_getch())
		{
		case 'a':
			dir = LEFT;
			break;
		case 'd':
			dir = RIGHT;
			break;
		case 'w':
			dir = UP;
			break;
		case 's':
			dir = DOWN;
			break;
		case 'b':
			dir = BOMB;
			break;
		case ' ':
			dir = SET;
			break;
		case 'E':
			dir = EXIT;
		}
	}
	void setCor()
	{
		int func = 0;
		switch (dir)
		{
		case EXIT:
			retGame = true;
			keyGame = true;
			keyPlay = false;
			system("cls");
		case LEFT:
			if (num2 - 1 >= 0) {
				CoorCur[num1][num2] = false;
				num2--;
				CoorCur[num1][num2] = true;
			}
			else
			{

			}
			break;
		case RIGHT:
			if (num2 + 1 < Column) {
				CoorCur[num1][num2] = false;
				num2++;
				CoorCur[num1][num2] = true;
			}
			else
			{

			}
			break;
		case UP:///
			if (num1 - 1 >= 0) {
				CoorCur[num1][num2] = false;
				num1--;
				CoorCur[num1][num2] = true;
			}
			else
			{

			}
			break;
		case DOWN:
			if (num1 + 1 < Line) {
				CoorCur[num1][num2] = false;
				num1++;
				CoorCur[num1][num2] = true;
			}
			else
			{

			}
			break;
		case BOMB:
			FuncArr[num1][num2] = 'B';
			break;
		case SET:
			if (ArrN[num1][num2] == 9)
			{
				system("cls");
				cout << "You lost! Do you want to return your move? 1-Yes Any-No" << endl;
				cin >> func;
				if (func == 1)
				{

				}
				else
				{
					keyGame = true;
				}
			}
			else if (ArrN[num1][num2] == 0)
			{
				OpenNullArr(num1, num2);
			}
			else
			{
				LettArr[num1][num2] = true;
			}
			break;
		}
	}
	void OpenNullArr(int num1, int num2)
	{
		if ((num1 >= 0) && (num1 < Line)) {
			if ((num2 >= 0) && (num2 < Column)) {

				if (LettArr[num1][num2] == false) {

					LettArr[num1][num2] = true;

					if (ArrN[num1][num2] == 0) {
						OpenNullArr(num1 - 1, num2 - 1);
						OpenNullArr(num1 - 1, num2);
						OpenNullArr(num1 - 1, num2 + 1);
						OpenNullArr(num1, num2 - 1);
						OpenNullArr(num1, num2 + 1);
						OpenNullArr(num1 + 1, num2 - 1);
						OpenNullArr(num1 + 1, num2);
						OpenNullArr(num1 + 1, num2 + 1);
					}
				}

			}
		}
	}
	void drawArr()
	{
		cout << "\t\t\t\t\t  ";
		for(int i =0;i<Line;i++)
		{
			cout << i << " ";
		}
		cout << endl;
		for (int i = 0; i < Line; i++)
		{
			cout << "\t\t\t\t\t"<<i<<" ";
			for (int j = 0; j < Column; j++)
			{
				
				if (CoorCur[i][j] == true)
				{
					SetConsoleTextAttribute(hConsole, 13);
					cout << "Y ";
					SetConsoleTextAttribute(hConsole, 7);
				}
				else
				{
					if (LettArr[i][j] == true)
					{
						if (ArrN[i][j] == 0)
						{
							SetConsoleTextAttribute(hConsole, 8);
						}
						else if (ArrN[i][j] == 1)
						{
							SetConsoleTextAttribute(hConsole, 1);
						}
						else if (ArrN[i][j] == 2)
						{
							SetConsoleTextAttribute(hConsole, 2);
						}
						else if (ArrN[i][j] == 3)
						{
							SetConsoleTextAttribute(hConsole, 4);
						}
						else if (ArrN[i][j] == 4)
						{
							SetConsoleTextAttribute(hConsole, 3);
						}
						else if (ArrN[i][j] == 5)
						{
							SetConsoleTextAttribute(hConsole, 5);
						}
						else
						{
							SetConsoleTextAttribute(hConsole, 8);
						}
						cout << ArrN[i][j] << " ";
						SetConsoleTextAttribute(hConsole, 7);
					}
					else
					{
						if (FuncArr[i][j] == 'B')
						{
							cout << "B ";
						}
						else
						{
							cout << "H ";
						}
					}
				}
			}
			cout << endl;
		}

	}
	void takeopenArr()
	{
	for(int i =0;i<Line;i++)
	{
		int openNum = 0;
		for (int j = 0; j < Column; j++)
		{
			if(LettArr[i][j]==true)
			{
				openNum = openNum + 1;
			}
		}
		int ib = 0;
		ib = (Line*Column) - NumBomb;
		if (ib == openNum)
		{
			keyGame = true;
			keywin = true;
		}
	}
	}
	/////////////////////////////////////////////////////////////////////////////
	void deleteArrs()
	{
		for (int count = 0; count < Line; count++)
			delete[]ArrN[count];
		for (int count = 0; count < Line; count++)
			delete[]LettArr[count];
		for (int count = 0; count < Line; count++)
			delete[]CoorCur[count];
		for (int count = 0; count < Line; count++)
			delete[]FuncArr[count];
	}
};
class User
{
public:
	void showMenu()
	{
		int func = 0;
		cout << "\t\t\t\t\tGame" << endl;
		cout << "\t\t\t\t\tOptions" << endl;
		cout << "\t\t\t\t\tExit" << endl;
		cin >> func;
		system("cls");
		switch(func)
		{
		case 0:

			break;
		case 1:
			showOptions();
			break;
		case 2:
			keyGame = true;
			keyGame = true;
			break;
		default:
			showMenu();
		}
	}
	void showOptions()
	{
		int func = 0;
		cout << "\t\t\t\t\tBeginner" << endl;
		cout << "\t\t\t\t\tCultivating" << endl;
		cout << "\t\t\t\t\tCostum" << endl;
		cout << "\t\t\t\t\tExit" << endl;
		cin >> func;
		system("cls");
		switch (func)
		{
		case 0:
			level = "Beginner";
			break;
		case 1:
			level = "Ñultivating";
			break;
		case 2:
			level = "Costum";
			break;
		case 3:
			showMenu();
			break;
		default:
			showOptions();
		}
	}
};
int main()
{
	int Line, Column, NumBomb;
	srand(time(0));
	while (keyPlay == false) {
		Game objGame;
		User objUser;
		objUser.showMenu();
		if (retGame == true) { retGame == false; }
		else {
			if (level == "Beginner")
			{
				Line = 10;
				Column = 10;
				NumBomb = 10;
			}
			else if (level == "Ñultivating")
			{
				Line = 15;
				Column = 15;
				NumBomb = 50;
			}
			else {
				int k = 0;
				while (k == 0)
				{
					cout << "Line: ";
					cin >> Line;
					cout << endl << "Column: ";
					cin >> Column;
					cout << endl << "Number of Bombs: ";
					cin >> NumBomb;
					cout << endl;
					if(Line>=5 && Column>=5 && NumBomb>0 && Line<=30 && Column<=30 && NumBomb<40 && Line*Column > NumBomb)
					{
						k = 1;
					}
					else
					{
						cout << "ERROR! Line>=5 Column>=5 Line<=30 Column<=30  NumBomb<40 Line*Column > NumBomb" << endl;
						system("pause");
					}
					system("cls");
				}
			}
			objGame.createArrs(NumBomb, Line, Column);
			objGame.setBomb();
			objGame.setNums();
			retGame == false;
		}
		
		while (keyGame == false)
		{
			objGame.drawArr();
			objGame.getFunction();
			objGame.setCor();
			objGame.takeopenArr();
			system("cls");
		}
		keyGame = false;
		if (retGame == false) {
			if (keywin == true)
				cout << "W I N N E R !" << endl;
			else 
				cout << "L O S T E R !" << endl;
			objGame.deleteArrs();
			keywin = false;
		}
	}
	system("pause");
	return 0;
}