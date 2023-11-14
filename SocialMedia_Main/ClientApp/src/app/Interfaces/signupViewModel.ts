export interface signupViewModel{
    firstName: string,
    lastName: string | null,
    email: string,
    password: string,
    confirmPassword: string,
    dateOfBirth: string,
    gender: '',
    phoneNumber : string,
    country: string,
    city: string
}

export enum genderType{
    Male,
    Female,
    Unknown
}

export interface signupResponseViewModel{
    isSuccess: boolean,
    userId: string,
    token: string,
    errorMessage: string[]
};



