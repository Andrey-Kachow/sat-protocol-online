from pydantic import BaseModel


class User(BaseModel):
    id: int
    username: str
    password: str  
    playeruid: str

    @classmethod
    def from_row(cls, row):
        return User(**row)
