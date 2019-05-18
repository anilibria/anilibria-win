#include "onlinevideomodel.h"

OnlineVideoModel::OnlineVideoModel()
{

}

void OnlineVideoModel::readFromApiModel(const QJsonObject &jsonObject)
{
    m_Id = jsonObject.value("id").toInt();
    m_Title = jsonObject.value("title").toString();
    m_SD = jsonObject.value("sd").toString();
    m_HD = jsonObject.value("hd").toString();
    m_FullHD = jsonObject.value("fullhd").toString();
    m_SourceHD = jsonObject.value("srcSd").toString();
    m_SourceHD = jsonObject.value("srcHd").toString();

}
