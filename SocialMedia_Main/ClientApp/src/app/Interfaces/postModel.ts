export interface addPostViewModel{
    postId: string,
    description?: string,
    fileUrl?: string | null,
    fileUrlType: string
}

export interface updatePostViewModel{
    postId: string,
    description?: string,
    fileUrl?: string | null,
    fileUrlType: string
}


export interface postViewModel{
    postId: string,
    title: string,
    description: string,
    parentId: string,
    parentName: string,
    data: object,
    created: string,
    updated: string,
    user: any,
    fileUrlType: string,
    fileUrl: string,
    totalLikes: number,
    isLikedByCurrentUser: boolean
}


