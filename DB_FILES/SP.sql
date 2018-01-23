DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `CREATE_UPDATE_RECORD`(
	p_ID		int, 
	p_docID	text, 
	p_docText	text, 
	p_Date	text, 
	p_Phone	text, 
	p_Organization text
)
BEGIN
	IF(p_ID = 0) THEN
    
		INSERT INTO crescent 
        (
			docID, docText,  Date , Phone, Organization
        )
        VALUES
        (
			p_docID, p_docText, p_Date, p_Phone, p_Organization 
		);
        
		SELECT LAST_INSERT_ID() INTO @LASTID;
        
        SELECT @LASTID AS ID;
    
    ELSE
    
		UPDATE crescent 
		SET docID = p_docID, docText = p_docText, Date = p_Date
			, Phone = p_Phone, Organization = p_Organization 
		where ID = p_ID;
    
		SELECT p_ID AS ID;
        
    END IF;
	
    
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `DELETE_RECORD`(
	p_ID	INT,
    p_IS_RETRIEVE	INT
)
BEGIN
	IF(p_IS_RETRIEVE = 1)THEN
    
		UPDATE crescent SET IS_DELETED = 0 WHERE ID = p_ID;
	
    ELSE 
    
		UPDATE crescent SET IS_DELETED = 1 WHERE ID = p_ID;
    
    END IF;

END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `GET_TABLE`()
BEGIN

    # TABLE 0 - DISTINCT DOCID
    SELECT DISTINCT docID FROM crescent 
    WHERE (docID != '' and (docID is not null)); #AND is_deleted = 0;
    
   /*
   # TABLE 1 - ALL RECORD
     SELECT ID, docID, c.docID as name, docText, Date, Phone, Organization 
	 FROM crescent c 
	 where (docID != '' and (docID is not null)) AND is_deleted = 0
     order by id desc;
	*/
    
    # TABLE 1 - ALL RECORD
	SELECT ID, C.docID, c.docID as name, C.docText, C.Date, C.Phone, C.Organization 
		, CASE WHEN S.docID IS NULL THEN 0 ELSE 1 END AS IS_SUSPECT
        , C.IS_DELETED
	FROM crescent c 
	LEFT JOIN suspects S ON S.docID = C.docID AND S.docText =  C.docText AND C.Date = S.Date
	AND S.Phone = C.Phone AND S.Misc = C.Misc
	AND S.Person = C.Person AND S.Location = C.Location AND S.Organization = C.Organization
	WHERE (C.docID != '' and (C.docID IS NOT NULL)) #AND IS_DELETED = 0
	ORDER BY id DESC;

END$$
DELIMITER ;
